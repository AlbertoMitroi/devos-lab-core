namespace Lab.Core.Workflow.EventFlow.Plugins;

/// <summary>Maps plugin class names to the BPMN element they handle.</summary>
public static class FlowPluginConventions
{
    public const string TaskSuffix = "Handler";
    public const string ActionSuffix = "ActionHandler";
    public const string NewFlowSuffix = "NewFlowHandler";

    public static string TaskIdOf(Type pluginType) => Strip(pluginType.Name, TaskSuffix);

    public static string ActionNameOf(Type pluginType) => Strip(pluginType.Name, ActionSuffix);

    public static string ProcessIdOf(Type pluginType) => Strip(pluginType.Name, NewFlowSuffix);

    private static string Strip(string name, string suffix) =>
        name.EndsWith(suffix, StringComparison.Ordinal)
            ? name[..^suffix.Length]
            : throw new InvalidOperationException($"Plugin type '{name}' does not end with '{suffix}'.");
}
