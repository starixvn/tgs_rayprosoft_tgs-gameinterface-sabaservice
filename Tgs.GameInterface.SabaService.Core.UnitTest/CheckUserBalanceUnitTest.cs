using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using System.Net;
using Tgs.GameInterface.SabaService.Core.Exceptions;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class CheckUserBalanceUnitTest
{
    [TestMethod]
    public async Task 玩家不存在()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        var walletType = GameWalletType.Sportsbook;

        var url = new Uri(apiUrl, "/api/CheckUserBalance");
        var response = "{\"error_code\":0,\"message\":\"\",\"Data\":[{\"vendor_member_id\":\"4ucmqplxb029\",\"balance\":null,\"bonus_balance\":null,\"outstanding\":null,\"currency\":null,\"error_code\":2}]}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .CheckUserBalanceAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorMemberId: vendorMemberId,
                walletType: walletType)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsFalse(actual.IsPlayerExist);
    }

    [TestMethod]
    public async Task 取得餘額()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        var walletType = GameWalletType.Sportsbook;

        var url = new Uri(apiUrl, "/api/CheckUserBalance");
        var response = "{\"error_code\":0,\"message\":\"\",\"Data\":[{\"vendor_member_id\":\"4ucmqplxb029\",\"balance\":100,\"bonus_balance\":0,\"outstanding\":0,\"currency\":0,\"error_code\":0}]}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .CheckUserBalanceAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorMemberId: vendorMemberId,
                walletType: walletType)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsPlayerExist);
        Assert.AreEqual(100m, actual.Balance);
    }

    [TestMethod]
    public async Task 其他錯誤()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        var walletType = GameWalletType.Sportsbook;

        var url = new Uri(apiUrl, "/api/CheckUserBalance");
        var response = "{\"error_code\":1,\"message\":\"Failed during executed\",\"Data\":[]}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        _ = await Assert
            .ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
                sut.CheckUserBalanceAsync(
                    apiUrl: apiUrl,
                    vendorId: vendorId,
                    vendorMemberId: vendorMemberId,
                    walletType: walletType)
                .AsTask())
            .ConfigureAwait(false);
    }
}