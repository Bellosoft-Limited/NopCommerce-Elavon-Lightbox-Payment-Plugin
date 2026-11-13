using System.Net;
using System.Text;

namespace Nop.Plugin.Payments.Elavon.Tests.Helpers;

/// <summary>
/// Stands in for the Elavon API: records the body of each request and answers creating an order and a payment session
/// </summary>
public class FakeElavonHttpMessageHandler : HttpMessageHandler
{
    public Dictionary<string, string> RequestBodies { get; } = [];

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var path = request.RequestUri!.AbsolutePath;
        RequestBodies[path] = request.Content == null ? null : await request.Content.ReadAsStringAsync(cancellationToken);

        var response = path switch
        {
            "/orders" => """{ "href": "https://elavon.test/orders/order-1" }""",
            "/payment-sessions" => """{ "id": "session-1" }""",
            _ => "{}"
        };

        return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(response, Encoding.UTF8, "application/json") };
    }
}
