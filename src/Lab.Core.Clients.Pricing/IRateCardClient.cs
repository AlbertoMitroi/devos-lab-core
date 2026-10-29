namespace Lab.Core.Clients.Pricing;

public interface IRateCardClient
{
    Task<RateCard?> GetRateCardAsync(string productCode, CancellationToken cancellationToken = default);

    Task<ResidualValue?> GetResidualValueAsync(string assetType, int termMonths, CancellationToken cancellationToken = default);
}
