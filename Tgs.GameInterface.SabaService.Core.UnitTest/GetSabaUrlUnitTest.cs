using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SabaService.Core.Exceptions;
using Tgs.GameInterface.SabaService.Core;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class GetSabaUrlUnitTest
{
    [TestMethod]
    public async Task 玩家不存在()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        var platformType = GamePlatformType.Desktop;

        var url = new Uri(apiUrl, "/api/GetSabaUrl");
        var response = "{\"error_code\":2,\"message\":\"member not found\",\"Data\":null}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameUrlApiClient>.Instance;

        var sut = new GameUrlApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetSabaUrlAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorMemberId: vendorMemberId,
                platformType: platformType)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsFalse(actual.IsPlayerExist);
    }

    [TestMethod]
    public async Task 取得遊戲網址()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        var platformType = GamePlatformType.Desktop;

        var url = new Uri(apiUrl, "/api/GetSabaUrl");
        var response = "{\"error_code\":0,\"message\":\"\",\"Data\":\"https://h0i2ob11.SABADomain.com/Deposit_ProcessLogin.aspx?token=xxxxxxx&lang=xx\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameUrlApiClient>.Instance;

        var sut = new GameUrlApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetSabaUrlAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorMemberId: vendorMemberId,
                platformType: platformType)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsPlayerExist);
        Assert.IsFalse(string.IsNullOrWhiteSpace(actual.Url));
    }

    [TestMethod]
    public async Task 其他錯誤()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        var platformType = GamePlatformType.Desktop;

        var url = new Uri(apiUrl, "/api/GetSabaUrl");
        var response = "{\"error_code\":1,\"message\":\"Failed during executed\",\"Data\":null}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameUrlApiClient>.Instance;

        var sut = new GameUrlApiClient(fakeHttpClient, fakeLog);

        _ = await Assert
            .ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
                sut.GetSabaUrlAsync(
                    apiUrl: apiUrl,
                    vendorId: vendorId,
                    vendorMemberId: vendorMemberId,
                    platformType: platformType)
                .AsTask())
            .ConfigureAwait(false);
    }
}
