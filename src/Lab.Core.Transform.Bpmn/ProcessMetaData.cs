namespace Lab.Core.Transform.Bpmn;

/// <summary>
/// Content of a <c>*.ProcessMetaData.json</c> file that sits next to a <c>.bpmn</c> file.
/// </summary>
public sealed class ProcessMetaData
{
    public string ProcessId { get; set; } = "";

    public string DisplayName { get; set; } = "";

    /// <summary>Flow event type names that start a new flow of this process.</summary>
    public List<string> StartedBy { get; set; } = [];

    /// <summary>End event id to the status the flow takes when it reaches that end event.</summary>
    public Dictionary<string, string> Statuses { get; set; } = [];

    /// <summary>Actions a user can run on a flow of this process, by action name.</summary>
    public Dictionary<string, ActionMetaData> Actions { get; set; } = [];

    /// <summary>Lane name to the work team that owns the user tasks of that lane.</summary>
    public Dictionary<string, string> Participants { get; set; } = [];
}

public sealed class ActionMetaData
{
    public string DisplayName { get; set; } = "";

    /// <summary>The action is available while one of these tasks is open; empty means always.</summary>
    public List<string> AvailableDuring { get; set; } = [];
}
