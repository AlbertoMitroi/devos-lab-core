using Microsoft.AspNetCore.SignalR;

namespace Lab.Core.MessageHubs;

public sealed class MessageHubUtility<THub>(IHubContext<THub> hubContext) : IMessageHubUtility
    where THub : Hub
{
    public Task SendToAll(string method, object payload, CancellationToken cancellationToken = default) =>
        hubContext.Clients.All.SendAsync(method, payload, cancellationToken);

    public Task SendToSpecifiedUsers(IReadOnlyCollection<string> userIds, string method, object payload, CancellationToken cancellationToken = default) =>
        hubContext.Clients.Users(userIds).SendAsync(method, payload, cancellationToken);
}
