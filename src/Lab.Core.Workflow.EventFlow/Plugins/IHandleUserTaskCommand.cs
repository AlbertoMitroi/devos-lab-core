namespace Lab.Core.Workflow.EventFlow.Plugins;

/// <summary>A command a user sends to complete an open user task of a flow.</summary>
public interface IUserTaskCommand
{
    Guid FlowId { get; }
}

/// <summary>
/// Completes a BPMN user task with a user's command. By convention the implementing class is named <c>{TaskId}Handler</c>.
/// </summary>
public interface IHandleUserTaskCommand<in TCommand>
    where TCommand : IUserTaskCommand
{
    Task<string?> Handle(TCommand command, FlowContext context, CancellationToken cancellationToken);
}
