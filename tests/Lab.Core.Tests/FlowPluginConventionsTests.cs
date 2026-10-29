using FluentAssertions;
using Lab.Core.Workflow.EventFlow.Plugins;

namespace Lab.Core.Tests;

public class FlowPluginConventionsTests
{
    private sealed class CheckHandler;

    private sealed class WithdrawActionHandler;

    private sealed class ReviewNewFlowHandler;

    [Fact]
    public void Plugin_names_map_to_bpmn_elements()
    {
        FlowPluginConventions.TaskIdOf(typeof(CheckHandler)).Should().Be("Check");
        FlowPluginConventions.ActionNameOf(typeof(WithdrawActionHandler)).Should().Be("Withdraw");
        FlowPluginConventions.ProcessIdOf(typeof(ReviewNewFlowHandler)).Should().Be("Review");
    }
}
