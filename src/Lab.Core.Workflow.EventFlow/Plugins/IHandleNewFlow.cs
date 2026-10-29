namespace Lab.Core.Workflow.EventFlow.Plugins;

/// <summary>
/// Initialises a new flow of a process. By convention the implementing class is named <c>{ProcessId}NewFlowHandler</c>.
/// </summary>
public interface IHandleNewFlow
{
    Task Initialize(FlowContext context, IReadOnlyDictionary<string, string> trigger, CancellationToken cancellationToken);
}
