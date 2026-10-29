namespace Lab.Core.Transform.Bpmn;

public enum NodeKind
{
    StartEvent,
    EndEvent,
    ServiceTask,
    UserTask,
    ExclusiveGateway,
    ParallelGateway,
}

public sealed record ProcessNode(string Id, string Name, NodeKind Kind, string? Lane);

public sealed record SequenceFlow(string Id, string SourceRef, string TargetRef, string? Condition, bool IsDefault);

public sealed record ProcessDefinition(
    string ProcessId,
    string Name,
    IReadOnlyList<ProcessNode> Nodes,
    IReadOnlyList<SequenceFlow> Flows,
    ProcessMetaData MetaData)
{
    public ProcessNode Node(string id) =>
        Nodes.FirstOrDefault(n => n.Id == id)
        ?? throw new InvalidOperationException($"Node '{id}' not found in process '{ProcessId}'.");

    public ProcessNode StartEvent => Nodes.Single(n => n.Kind == NodeKind.StartEvent);

    public IEnumerable<SequenceFlow> Outgoing(string nodeId) => Flows.Where(f => f.SourceRef == nodeId);
}
