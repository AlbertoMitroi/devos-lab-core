namespace Lab.Core.Workflow.EventFlow;

public sealed record CompletedTask(string TaskId, string? Outcome);

/// <summary>
/// State of one flow (one process instance), rebuilt from its event stream.
/// The <c>Apply</c> methods follow the self-aggregating snapshot convention of event stores such as Marten.
/// </summary>
public sealed class FlowAggregate
{
    public Guid Id { get; set; }

    public string ProcessId { get; set; } = "";

    public Guid? ParentFlowId { get; set; }

    public string Status { get; set; } = FlowStatuses.Started;

    public int Version { get; set; }

    public Dictionary<string, string> Data { get; set; } = [];

    public List<CompletedTask> CompletedTasks { get; set; } = [];

    public List<string> ExecutedActions { get; set; } = [];

    public void Apply(FlowStarted e)
    {
        Id = e.FlowId;
        ProcessId = e.ProcessId;
        ParentFlowId = e.ParentFlowId;
        Status = FlowStatuses.Started;
        foreach (var (key, value) in e.Data)
        {
            Data[key] = value;
        }
    }

    public void Apply(TaskCompleted e) => CompletedTasks.Add(new CompletedTask(e.TaskId, e.Outcome));

    public void Apply(ActionExecuted e) => ExecutedActions.Add(e.ActionName);

    public void Apply(FlowDataUpdated e)
    {
        foreach (var (key, value) in e.Data)
        {
            Data[key] = value;
        }
    }

    public void Apply(FlowStatusChanged e) => Status = e.Status;

    public void Apply(FlowFactRaised e)
    {
    }
}

public static class FlowStatuses
{
    public const string Started = "Started";
    public const string InProgress = "InProgress";
    public const string Withdrawn = "Withdrawn";
}
