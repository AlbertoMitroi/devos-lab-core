using System.Text.Json;
using System.Xml.Linq;

namespace Lab.Core.Transform.Bpmn;

/// <summary>
/// Reads a BPMN 2.0 file (as drawn with bpmn.io) and its <c>*.ProcessMetaData.json</c>.
/// Only the elements the Flow Engine executes are kept.
/// </summary>
public sealed class BpmnProcessLoader
{
    public const string MetaDataSuffix = ".ProcessMetaData.json";

    private static readonly XNamespace Bpmn = "http://www.omg.org/spec/BPMN/20100524/MODEL";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public ProcessDefinition LoadFile(string bpmnPath)
    {
        var metaDataPath = Path.ChangeExtension(bpmnPath, null) + MetaDataSuffix;
        var metaData = File.Exists(metaDataPath)
            ? JsonSerializer.Deserialize<ProcessMetaData>(File.ReadAllText(metaDataPath), JsonOptions)!
            : new ProcessMetaData();
        return Load(XDocument.Load(bpmnPath), metaData);
    }

    public IReadOnlyList<ProcessDefinition> LoadDirectory(string directory) =>
        Directory.EnumerateFiles(directory, "*.bpmn", SearchOption.AllDirectories)
            .Order(StringComparer.Ordinal)
            .Select(LoadFile)
            .ToList();

    public ProcessDefinition Load(XDocument document, ProcessMetaData metaData)
    {
        var process = document.Descendants(Bpmn + "process").Single();
        var processId = (string)process.Attribute("id")!;

        var lanes = process.Descendants(Bpmn + "lane")
            .SelectMany(lane => lane.Elements(Bpmn + "flowNodeRef")
                .Select(r => (NodeId: r.Value.Trim(), Lane: (string?)lane.Attribute("name"))))
            .ToDictionary(x => x.NodeId, x => x.Lane);

        var nodes = new List<ProcessNode>();
        foreach (var element in process.Elements())
        {
            NodeKind? kind = element.Name.LocalName switch
            {
                "startEvent" => NodeKind.StartEvent,
                "endEvent" => NodeKind.EndEvent,
                "serviceTask" => NodeKind.ServiceTask,
                "userTask" => NodeKind.UserTask,
                "exclusiveGateway" => NodeKind.ExclusiveGateway,
                "parallelGateway" => NodeKind.ParallelGateway,
                _ => null,
            };
            if (kind is null)
            {
                continue;
            }

            var id = (string)element.Attribute("id")!;
            nodes.Add(new ProcessNode(id, (string?)element.Attribute("name") ?? id, kind.Value, lanes.GetValueOrDefault(id)));
        }

        var defaults = process.Elements()
            .Select(e => (string?)e.Attribute("default"))
            .Where(d => d is not null)
            .ToHashSet();

        var flows = process.Elements(Bpmn + "sequenceFlow")
            .Select(f =>
            {
                var id = (string)f.Attribute("id")!;
                var condition = f.Element(Bpmn + "conditionExpression")?.Value.Trim();
                return new SequenceFlow(id, (string)f.Attribute("sourceRef")!, (string)f.Attribute("targetRef")!, condition, defaults.Contains(id));
            })
            .ToList();

        if (string.IsNullOrEmpty(metaData.ProcessId))
        {
            metaData.ProcessId = processId;
        }

        return new ProcessDefinition(processId, (string?)process.Attribute("name") ?? processId, nodes, flows, metaData);
    }
}
