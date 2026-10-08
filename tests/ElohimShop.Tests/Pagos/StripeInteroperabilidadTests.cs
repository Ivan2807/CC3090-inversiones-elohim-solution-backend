using System.Reflection;
using System.Text.Json;
using ElohimShop.Application.Pagos;
using ElohimShop.Domain.Platform;
using ElohimShop.Infrastructure.Pagos;
using ElohimShop.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ElohimShop.Tests.Pagos;

public class StripeInteroperabilidadTests
{
    [Fact]
    public void DtoDePago_SeSerializaConContratoEsperado()
    {
        var created = new PaymentIntentCreadoDto
        {
            ClientSecret = "pi_1234567890_secret_abcdef",
            ReservacionId = "res_123",
            MontoCentavos = 250000,
            Moneda = "gtq"
        };

        var estado = new PagoEstadoDto
        {
            PaymentIntentId = "pi_1234567890",
            Status = "succeeded",
            ReservacionId = "res_123",
            MontoCentavos = 250000,
            Moneda = "gtq"
        };

        var reembolso = new ReembolsoCreadoDto
        {
            ReembolsoId = "re_123",
            Estado = "succeeded",
            MontoCentavos = 250000,
            Moneda = "gtq"
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        using var createdDoc = JsonDocument.Parse(JsonSerializer.Serialize(created, jsonOptions));
        Assert.Equal("pi_1234567890_secret_abcdef", createdDoc.RootElement.GetProperty("clientSecret").GetString());
        Assert.Equal("res_123", createdDoc.RootElement.GetProperty("reservacionId").GetString());
        Assert.Equal(250000, createdDoc.RootElement.GetProperty("montoCentavos").GetInt64());
        Assert.Equal("gtq", createdDoc.RootElement.GetProperty("moneda").GetString());

        using var statusDoc = JsonDocument.Parse(JsonSerializer.Serialize(estado, jsonOptions));
        Assert.Equal("pi_1234567890", statusDoc.RootElement.GetProperty("paymentIntentId").GetString());
        Assert.Equal("succeeded", statusDoc.RootElement.GetProperty("status").GetString());

        using var refundDoc = JsonDocument.Parse(JsonSerializer.Serialize(reembolso, jsonOptions));
        Assert.Equal("re_123", refundDoc.RootElement.GetProperty("reembolsoId").GetString());
        Assert.Equal("succeeded", refundDoc.RootElement.GetProperty("estado").GetString());
    }

    [Fact]
    public void WebhookYEstadosStripe_SeNormalizanEnFormatoApiEsperado()
    {
        using var eventDoc = JsonDocument.Parse(LeerFixture("stripe_webhook_payment_intent_succeeded.json"));
        var payload = eventDoc.RootElement;

        Assert.Equal("payment_intent.succeeded", payload.GetProperty("type").GetString());
        var metadata = payload.GetProperty("data").GetProperty("object").GetProperty("metadata");
        Assert.Equal("res_123", metadata.GetProperty("reservacion_id").GetString());

        var method = typeof(StripePagosService).GetMethod("MapearEstadoParaApi", BindingFlags.NonPublic | BindingFlags.Static);
        Assert.NotNull(method);

        Assert.Equal("succeeded", method.Invoke(null, ["succeeded"]) as string);
        Assert.Equal("requires_payment_method", method.Invoke(null, ["requires_action"]) as string);
        Assert.Equal("processing", method.Invoke(null, ["processing"]) as string);
    }

    [Fact]
    public async Task CrearPaymentIntentAsync_SinConfiguracionStripe_DevuelveErrorEstandarizado()
    {
        var dbContext = new ElohimShopDbContext(
            new DbContextOptionsBuilder<ElohimShopDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options);

        var tenantProvider = new Mock<ITenantProvider>();
        tenantProvider.Setup(x => x.GetTenantId()).Returns("tenant-1");

        var platformContext = new PlatformDbContext(
            new DbContextOptionsBuilder<PlatformDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options,
            tenantProvider.Object);

        var service = new StripePagosService(
            dbContext,
            platformContext,
            tenantProvider.Object,
            Options.Create(new StripePaymentOptions { DefaultCurrency = "gtq" }));

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            service.CrearPaymentIntentAsync(
                "cliente-123",
                new CrearPaymentIntentDto { ReservacionId = "res_123" }));

        Assert.Contains("STRIPE_SECRET_KEY", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void StripeErrorResponse_Fixtures_ContienenFormatoEsperado()
    {
        using var errorDoc = JsonDocument.Parse(LeerFixture("stripe_error_response.json"));
        var error = errorDoc.RootElement.GetProperty("error");

        Assert.Equal("card_error", error.GetProperty("type").GetString());
        Assert.Equal("card_declined", error.GetProperty("code").GetString());
        Assert.Equal("Your card was declined.", error.GetProperty("message").GetString());
    }

    [Fact]
    public void FixtureDeContrato_CumpleConFormatoEsperado()
    {
        using var successDoc = JsonDocument.Parse(LeerFixture("payment_intent_created_success.json"));
        var root = successDoc.RootElement;

        Assert.Equal("pi_1234567890_secret_abcdef", root.GetProperty("clientSecret").GetString());
        Assert.Equal("res_123", root.GetProperty("reservacionId").GetString());
        Assert.Equal(250000, root.GetProperty("montoCentavos").GetInt64());
        Assert.Equal("gtq", root.GetProperty("moneda").GetString());
    }

    private static string LeerFixture(string nombre)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "Pagos", "Fixtures", nombre);
        return File.ReadAllText(path);
    }
}
