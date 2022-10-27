using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class KickUserUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorMemberId = "tg_4ucmqplxb030_test";

        var url = new Uri(apiUrl, "/api/KickUser/");
        var response = "{\"error_code\":0,\"message\":\"Successfully executed\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        await sut
           .KickUserAsync(
               apiUrl: apiUrl,
               vendorId: vendorId,
               vendorMemberId: vendorMemberId)
           .ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Player_Does_Not_Exist()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorMemberId = "tg_4ucmqplxb029_test";

        var url = new Uri(apiUrl, "/api/KickUser/");
        var response = "{\"error_code\":2,\"message\":\"User Not Found\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        await sut
           .KickUserAsync(
               apiUrl: apiUrl,
               vendorId: vendorId,
               vendorMemberId: vendorMemberId)
           .ConfigureAwait(false);
    }

    [TestMethod]
    public async Task Online()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorMemberId = "tg_4ucmqplxb029_test";

        var url = new Uri(apiUrl, "/api/KickUser/");
        var response = "{\"error_code\":3,\"message\":\"User is not online\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        await sut
           .KickUserAsync(
               apiUrl: apiUrl,
               vendorId: vendorId,
               vendorMemberId: vendorMemberId)
           .ConfigureAwait(false);
    }
}
