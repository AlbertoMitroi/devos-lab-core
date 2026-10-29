using FluentAssertions;
using Lab.Core.Transform.Bpmn;
using Lab.Core.Workflow.EventFlow;

namespace Lab.Core.Tests;

public class EventFlowAnalyzerTests
{
    private static readonly ProcessDefinition Process =
        new BpmnProcessLoader().LoadFile(Path.Combine(AppContext.BaseDirectory, "Processes", "Review.bpmn"));

    private static FlowAggregate Flow(params TaskCompleted[] completed)
    {
        var flow = new FlowAggregate();
        var id = Guid.NewGuid();
        flow.Apply(new FlowStarted(id, "Review", null, "ReviewRequested", []));
        foreach (var e in completed)
        {
            flow.Apply(e with { FlowId = id });
        }

        return flow;
    }

    [Fact]
    public void New_flow_has_the_first_service_task_open()
    {
        var analysis = new EventFlowAnalyzer().Analyze(Process, Flow());

        analysis.OpenTasks.Select(t => t.TaskId).Should().Equal("Check");
        analysis.Status.Should().Be(FlowStatuses.Started);
        analysis.AvailableActions.Should().BeEmpty();
    }

    [Fact]
    public void User_task_is_open_after_the_service_task_with_its_team_and_actions()
    {
        var analysis = new EventFlowAnalyzer().Analyze(Process, Flow(new TaskCompleted(Guid.Empty, "Check", null, null)));

        analysis.OpenTasks.Should().ContainSingle().Which.Should().Be(new OpenTask("Decide", "Decide", NodeKind.UserTask, "Reviewer", "ReviewTeam"));
        analysis.Status.Should().Be(FlowStatuses.InProgress);
        analysis.AvailableActions.Should().Equal("Withdraw");
    }

    [Theory]
    [InlineData("Accepted", "End_Accepted", "ReviewAccepted")]
    [InlineData("Rejected", "End_Rejected", "ReviewRejected")]
    [InlineData(null, "End_Rejected", "ReviewRejected")]
    public void Gateway_follows_the_outcome_or_the_default_flow(string? outcome, string end, string status)
    {
        var analysis = new EventFlowAnalyzer().Analyze(
            Process,
            Flow(new TaskCompleted(Guid.Empty, "Check", null, null), new TaskCompleted(Guid.Empty, "Decide", outcome, "user-1")));

        analysis.IsCompleted.Should().BeTrue();
        analysis.ReachedEndEvent.Should().Be(end);
        analysis.Status.Should().Be(status);
        analysis.OpenTasks.Should().BeEmpty();
    }

    [Fact]
    public void Withdrawn_flow_has_no_open_tasks_or_actions()
    {
        var flow = Flow(new TaskCompleted(Guid.Empty, "Check", null, null));
        flow.Apply(new FlowStatusChanged(flow.Id, FlowStatuses.Withdrawn));

        var analysis = new EventFlowAnalyzer().Analyze(Process, flow);

        analysis.OpenTasks.Should().BeEmpty();
        analysis.AvailableActions.Should().BeEmpty();
        analysis.IsCompleted.Should().BeTrue();
        analysis.Status.Should().Be(FlowStatuses.Withdrawn);
    }
}
