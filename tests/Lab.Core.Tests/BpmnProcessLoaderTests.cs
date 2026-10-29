using FluentAssertions;
using Lab.Core.Transform.Bpmn;

namespace Lab.Core.Tests;

public class BpmnProcessLoaderTests
{
    private static string ProcessFile(string name) => Path.Combine(AppContext.BaseDirectory, "Processes", name);

    [Fact]
    public void Loads_nodes_flows_lanes_and_metadata()
    {
        var process = new BpmnProcessLoader().LoadFile(ProcessFile("Review.bpmn"));

        process.ProcessId.Should().Be("Review");
        process.Nodes.Select(n => (n.Id, n.Kind)).Should().Contain([
            ("Start", NodeKind.StartEvent),
            ("Check", NodeKind.ServiceTask),
            ("Decide", NodeKind.UserTask),
            ("Gateway_Decision", NodeKind.ExclusiveGateway),
        ]);
        process.Node("Decide").Lane.Should().Be("Reviewer");
        process.Flows.Single(f => f.Id == "Flow_Rejected").IsDefault.Should().BeTrue();
        process.Flows.Single(f => f.Id == "Flow_Accepted").Condition.Should().Be("Accepted");
        process.MetaData.StartedBy.Should().Equal("ReviewRequested");
    }

    [Fact]
    public void Loads_every_process_of_a_directory()
    {
        var processes = new BpmnProcessLoader().LoadDirectory(Path.Combine(AppContext.BaseDirectory, "Processes"));

        processes.Select(p => p.ProcessId).Should().Equal("Review");
    }
}
