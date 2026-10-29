namespace Lab.Core.Workflow.EventFlow.Plugins;

/// <summary>
/// Runs a BPMN service task. By convention the implementing class is named <c>{TaskId}Handler</c>
/// and lives in a file of the same name; see <see cref="FlowPluginConventions"/>.
/// </summary>
public interface IHandleServiceTask
{
    /// <returns>The outcome of the task, matched against the conditions of the next exclusive gateway.</returns>
    Task<string?> Handle(FlowContext context, CancellationToken cancellationToken);
}
