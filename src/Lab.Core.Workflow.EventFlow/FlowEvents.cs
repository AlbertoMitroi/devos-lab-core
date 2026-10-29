namespace Lab.Core.Workflow.EventFlow;

/// <summary>Base type of every event appended to a flow stream.</summary>
public abstract record FlowEvent(Guid FlowId);

public sealed record FlowStarted(Guid FlowId, string ProcessId, Guid? ParentFlowId, string TriggeredBy, Dictionary<string, string> Data)
    : FlowEvent(FlowId);

public sealed record TaskCompleted(Guid FlowId, string TaskId, string? Outcome, string? CompletedBy) : FlowEvent(FlowId);

public sealed record ActionExecuted(Guid FlowId, string ActionName, string? ExecutedBy) : FlowEvent(FlowId);

public sealed record FlowDataUpdated(Guid FlowId, Dictionary<string, string> Data) : FlowEvent(FlowId);

public sealed record FlowStatusChanged(Guid FlowId, string Status) : FlowEvent(FlowId);

/// <summary>A named business fact raised by a plugin; processes list fact names in <c>StartedBy</c> to be spawned by it.</summary>
public sealed record FlowFactRaised(Guid FlowId, string Fact, Dictionary<string, string> Data) : FlowEvent(FlowId);
