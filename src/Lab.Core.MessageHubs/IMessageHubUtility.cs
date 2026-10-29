namespace Lab.Core.MessageHubs;

/// <summary>Pushes messages from the backend to connected front-end clients.</summary>
public interface IMessageHubUtility
{
    Task SendToAll(string method, object payload, CancellationToken cancellationToken = default);

    Task SendToSpecifiedUsers(IReadOnlyCollection<string> userIds, string method, object payload, CancellationToken cancellationToken = default);
}
