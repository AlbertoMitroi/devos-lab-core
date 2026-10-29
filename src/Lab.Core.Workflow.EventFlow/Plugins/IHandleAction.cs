namespace Lab.Core.Workflow.EventFlow.Plugins;

/// <summary>
/// Runs an action declared in the process metadata. By convention the implementing class is named <c>{ActionName}ActionHandler</c>.
/// </summary>
public interface IHandleAction
{
    Task Handle(FlowContext context, CancellationToken cancellationToken);
}
