using System.Net;
using System.Text;
using System.Text.Json;

namespace EP.Tamin.NET.Tests;

public class TaminSessionTests
{
    [Fact]
    public void Constructor_WhenTokenRequiredWithoutToken_Throws()
    {
        var client = new HttpClient(new StubHandler());

        Assert.Throws<AuthTokenNotSuppliedException>(() => new TaminSession(client));
    }

    [Fact]
    public async Task ServiceClient_UsesExpectedEndpointAndQuery()
    {
        HttpRequestMessage? captured = null;
        var handler = new StubHandler((request, _) =>
        {
            captured = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\":{\"list\":[{\"id\":1}]}}", Encoding.UTF8, "application/json")
            });
        });

        var session = new TaminSession(new HttpClient(handler), "token");
        var result = await session.Service.GetAllServicesAsync(new Dictionary<string, string?> { ["serviceType"] = "17" });

        Assert.NotNull(captured);
        Assert.Equal("https://ep-test.tamin.ir/api/ws-services?serviceType=17", captured!.RequestUri!.ToString());
        Assert.Equal(JsonValueKind.Array, result.ValueKind);
        Assert.Equal(1, result[0].GetProperty("id").GetInt32());
    }

    [Fact]
    public async Task PrescriptionClient_UsesExpectedEndpointForCreate()
    {
        HttpRequestMessage? captured = null;
        var handler = new StubHandler((request, _) =>
        {
            captured = request;
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\":{\"list\":[{\"ok\":true}]}}", Encoding.UTF8, "application/json")
            });
        });

        var session = new TaminSession(new HttpClient(handler), "token");
        var result = await session.Prescription.CreatePrescriptionAsync(new { patient = "x" });

        Assert.NotNull(captured);
        Assert.Equal(HttpMethod.Post, captured!.Method);
        Assert.Equal("https://ep-test.tamin.ir/api/interface/epresc/SendEpresc", captured.RequestUri!.ToString());
        Assert.True(result[0].GetProperty("ok").GetBoolean());
    }

    [Fact]
    public async Task ServiceClient_On400_ThrowsBadRequest()
    {
        var handler = new StubHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent("{\"error\":\"bad\"}", Encoding.UTF8, "application/json")
        }));

        var session = new TaminSession(new HttpClient(handler), "token");

        await Assert.ThrowsAsync<BadRequest>(() => session.Service.GetAllServicesAsync());
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handler;

        public StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>? handler = null)
        {
            _handler = handler ?? ((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\":{\"list\":[]}}", Encoding.UTF8, "application/json")
            }));
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => _handler(request, cancellationToken);
    }
}
