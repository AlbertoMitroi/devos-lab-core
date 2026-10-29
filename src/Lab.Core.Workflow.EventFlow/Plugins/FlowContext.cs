namespace Lab.Core.Workflow.EventFlow.Plugins;

/// <summary>What a plugin sees of the flow it runs in, and where it records new events.</summary>
public sealed class FlowContext(FlowAggregate flow, string? userId)
{
    private readonly List<FlowEvent> _newEvents = [];

    public Guid FlowId => flow.Id;

    public string ProcessId => flow.ProcessId;

    public IReadOnlyDictionary<string, string> Data => flow.Data;

    public string? UserId => userId;

    public IReadOnlyList<FlowEvent> NewEvents => _newEvents;

    public void SetData(string key, string value) =>
        _newEvents.Add(new FlowDataUpdated(flow.Id, new Dictionary<string, string> { [key] = value }));

    public void ChangeStatus(string status) => _newEvents.Add(new FlowStatusChanged(flow.Id, status));

    public void RaiseFact(string fact, Dictionary<string, string>? data = null) =>
        _newEvents.Add(new FlowFactRaised(flow.Id, fact, data ?? []));
}
