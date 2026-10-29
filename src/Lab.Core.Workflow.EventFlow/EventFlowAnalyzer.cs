using Lab.Core.Transform.Bpmn;

namespace Lab.Core.Workflow.EventFlow;

public sealed record OpenTask(string TaskId, string Name, NodeKind Kind, string? Lane, string? WorkTeam);

public sealed record FlowAnalysis(
    string ProcessId,
    IReadOnlyList<OpenTask> OpenTasks,
    IReadOnlyList<string> AvailableActions,
    string Status,
    bool IsCompleted,
    string? ReachedEndEvent);

/// <summary>
/// Walks the BPMN graph of a process with the completed tasks of a flow and returns
/// the open tasks, the actions a user may run and the status of the flow.
/// </summary>
public sealed class EventFlowAnalyzer
{
    public FlowAnalysis Analyze(ProcessDefinition process, FlowAggregate flow)
    {
        if (flow.Status == FlowStatuses.Withdrawn)
        {
            return new FlowAnalysis(process.ProcessId, [], [], FlowStatuses.Withdrawn, true, null);
        }

        var outcomes = new Dictionary<string, string?>();
        foreach (var task in flow.CompletedTasks)
        {
            outcomes[task.TaskId] = task.Outcome;
        }

        var open = new List<OpenTask>();
        string? reachedEnd = null;
        var visited = new HashSet<string>();
        var pending = new Queue<(string NodeId, string? Outcome)>();
        pending.Enqueue((process.StartEvent.Id, null));

        while (pending.Count > 0)
        {
            var (nodeId, outcome) = pending.Dequeue();
            if (!visited.Add(nodeId))
            {
                continue;
            }

            var node = process.Node(nodeId);
            switch (node.Kind)
            {
                case NodeKind.StartEvent:
                case NodeKind.ParallelGateway:
                    foreach (var f in process.Outgoing(nodeId))
                    {
                        pending.Enqueue((f.TargetRef, outcome));
                    }

                    break;

                case NodeKind.ServiceTask:
                case NodeKind.UserTask:
                    if (outcomes.TryGetValue(nodeId, out var taskOutcome))
                    {
                        foreach (var f in process.Outgoing(nodeId))
                        {
                            pending.Enqueue((f.TargetRef, taskOutcome));
                        }
                    }
                    else
                    {
                        var team = node.Lane is null ? null : process.MetaData.Participants.GetValueOrDefault(node.Lane);
                        open.Add(new OpenTask(node.Id, node.Name, node.Kind, node.Lane, team));
                    }

                    break;

                case NodeKind.ExclusiveGateway:
                    var next = ChooseFlow(process.Outgoing(nodeId).ToList(), outcome);
                    if (next is not null)
                    {
                        pending.Enqueue((next.TargetRef, outcome));
                    }

                    break;

                case NodeKind.EndEvent:
                    reachedEnd = nodeId;
                    break;
            }
        }

        var completed = reachedEnd is not null && open.Count == 0;
        var status = completed
                ? process.MetaData.Statuses.GetValueOrDefault(reachedEnd!) ?? process.Node(reachedEnd!).Name
                : open.Count > 0 && flow.CompletedTasks.Count > 0 ? FlowStatuses.InProgress : flow.Status;

        var openIds = open.Select(t => t.TaskId).ToHashSet();
        var actions = completed
            ? []
            : process.MetaData.Actions
                .Where(a => a.Value.AvailableDuring.Count == 0 || a.Value.AvailableDuring.Any(openIds.Contains))
                .Select(a => a.Key)
                .Order(StringComparer.Ordinal)
                .ToList();

        return new FlowAnalysis(process.ProcessId, open, actions, status, completed, reachedEnd);
    }

    private static SequenceFlow? ChooseFlow(IReadOnlyList<SequenceFlow> outgoing, string? outcome) =>
        outgoing.FirstOrDefault(f => f.Condition is not null && string.Equals(f.Condition, outcome, StringComparison.Ordinal))
        ?? outgoing.FirstOrDefault(f => f.IsDefault)
        ?? (outgoing.Count == 1 ? outgoing[0] : null);
}
