using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;

namespace Lab.Core.MessageHubs;

public static class MessageHubExtensions
{
    /// <summary>Registers <see cref="IMessageHubUtility"/> over the hub <typeparamref name="THub"/>.</summary>
    public static IServiceCollection AddMessageHubUtility<THub>(this IServiceCollection services)
        where THub : Hub
    {
        services.AddSingleton<IMessageHubUtility, MessageHubUtility<THub>>();
        return services;
    }
}
