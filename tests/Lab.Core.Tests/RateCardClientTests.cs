using System.Net;
using System.Text;
using FluentAssertions;
using Lab.Core.Clients.Pricing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Lab.Core.Tests;

public class RateCardClientTests
{
    private sealed class StubHandler(Func<HttpRequestMessage, HttpResponseMessage> respond) : HttpMessageHandler
    {
        public List<Uri> Requests { get; } = [];

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Requests.Add(request.RequestUri!);
            return Task.FromResult(respond(request));
        }
    }

    private static (IRateCardClient Client, StubHandler Handler) Create(Func<HttpRequestMessage, HttpResponseMessage> respond)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Clients:RateCards:BaseAddress"] = "http://pricing.test/" })
            .Build();
        var handler = new StubHandler(respond);
        var services = new ServiceCollection();
        services.AddRateCardClient(configuration).ConfigurePrimaryHttpMessageHandler(() => handler);
        return (services.BuildServiceProvider().GetRequiredService<IRateCardClient>(), handler);
    }

    private static HttpResponseMessage Json(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8, "application/json") };

    [Fact]
    public async Task Gets_a_rate_card_by_product_code()
    {
        var (client, handler) = Create(_ => Json("""{"productCode":"EQ-36","baseRatePercent":4.5,"minTermMonths":12,"maxTermMonths":60}"""));

        var card = await client.GetRateCardAsync("EQ-36");

        card.Should().Be(new RateCard("EQ-36", 4.5m, 12, 60));
        handler.Requests.Single().AbsoluteUri.Should().Be("http://pricing.test/api/rate-cards/EQ-36");
    }

    [Fact]
    public async Task Missing_rate_card_is_null()
    {
        var (client, _) = Create(_ => new HttpResponseMessage(HttpStatusCode.NotFound));

        (await client.GetRateCardAsync("NONE")).Should().BeNull();
    }

    [Fact]
    public async Task Gets_a_residual_value_with_query_parameters()
    {
        var (client, handler) = Create(_ => Json("""{"assetType":"forklift","termMonths":36,"percent":22.5}"""));

        var value = await client.GetResidualValueAsync("forklift", 36);

        value.Should().Be(new ResidualValue("forklift", 36, 22.5m));
        handler.Requests.Single().PathAndQuery.Should().Be("/api/residual-values?assetType=forklift&termMonths=36");
    }
}
