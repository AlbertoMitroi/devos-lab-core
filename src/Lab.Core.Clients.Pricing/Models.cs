namespace Lab.Core.Clients.Pricing;

public sealed record RateCard(string ProductCode, decimal BaseRatePercent, int MinTermMonths, int MaxTermMonths);

public sealed record ResidualValue(string AssetType, int TermMonths, decimal Percent);
