using System.Net;
using System.Net.Http.Json;

namespace Lab.Core.Clients.Pricing;

internal sealed class RateCardClient(HttpClient httpClient) : IRateCardClient
{
    public async Task<RateCard?> GetRateCardAsync(string productCode, CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.GetAsync($"api/rate-cards/{Uri.EscapeDataString(productCode)}", cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RateCard>(cancellationToken);
    }

    public Task<ResidualValue?> GetResidualValueAsync(string assetType, int termMonths, CancellationToken cancellationToken = default) =>
        httpClient.GetFromJsonAsync<ResidualValue>(
            $"api/residual-values?assetType={Uri.EscapeDataString(assetType)}&termMonths={termMonths}",
            cancellationToken);
}
