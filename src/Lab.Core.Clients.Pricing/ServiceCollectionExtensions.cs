using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lab.Core.Clients.Pricing;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IRateCardClient"/>; the base address comes from <c>Clients:RateCards:BaseAddress</c>.
    /// </summary>
    public static IHttpClientBuilder AddRateCardClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RateCardClientOptions>(configuration.GetSection(RateCardClientOptions.SectionName));
        return services.AddHttpClient<IRateCardClient, RateCardClient>((provider, client) =>
        {
            var options = provider.GetRequiredService<IOptions<RateCardClientOptions>>().Value;
            client.BaseAddress = options.BaseAddress
                ?? throw new InvalidOperationException($"{RateCardClientOptions.SectionName}:BaseAddress is not configured.");
        });
    }
}
