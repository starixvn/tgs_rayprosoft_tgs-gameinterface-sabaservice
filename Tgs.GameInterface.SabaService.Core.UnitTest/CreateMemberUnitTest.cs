using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SabaService.Core.Exceptions;
using Tgs.GameInterface.SabaService.Core;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class CreateMemberUnitTest
{
    [TestMethod]
    public async Task 玩家已經存在()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        string operatorId = "ZF055PMW_develop";
        string firstName = "GPI";
        string lastName = "4ucmqplxb029";
        int oddsType = 2;
        int currency = 13;
        decimal maxTransfer = 9999999999;
        decimal minTransfer = 0;

        var url = new Uri(apiUrl, "/api/CreateMember/");
        var response = "{\"error_code\":2,\"message\":\"username dupliate or use unsupported characters\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        await sut
            .CreateMemberAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorMemberId: vendorMemberId,
                operatorId: operatorId,
                firstName: firstName,
                lastName: lastName,
                oddsType: oddsType,
                currency: currency,
                maxTransfer: maxTransfer,
                minTransfer: minTransfer)
            .ConfigureAwait(false);
    }

    [TestMethod]
    public async Task 創建玩家成功()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        string operatorId = "ZF055PMW_develop";
        string firstName = "GPI";
        string lastName = "4ucmqplxb029";
        int oddsType = 2;
        int currency = 13;
        decimal maxTransfer = 9999999999;
        decimal minTransfer = 0;

        var url = new Uri(apiUrl, "/api/CreateMember/");
        var response = "{\"error_code\":0,\"message\":\"Successfully executed\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        await sut
            .CreateMemberAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorMemberId: vendorMemberId,
                operatorId: operatorId,
                firstName: firstName,
                lastName: lastName,
                oddsType: oddsType,
                currency: currency,
                maxTransfer: maxTransfer,
                minTransfer: minTransfer)
            .ConfigureAwait(false);
    }

    [TestMethod]
    public async Task 其他錯誤()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "kwmmuz6qqi";
        var vendorMemberId = "4ucmqplxb029";
        string operatorId = "ZF055PMW_develop";
        string firstName = "GPI";
        string lastName = "4ucmqplxb029";
        int oddsType = 2;
        int currency = 13;
        decimal maxTransfer = 9999999999;
        decimal minTransfer = 0;

        var url = new Uri(apiUrl, "/api/CreateMember/");
        var response = "{\"error_code\":1,\"message\":\"Failed during executed\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        _ = await Assert
            .ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
                sut.CreateMemberAsync(
                    apiUrl: apiUrl,
                    vendorId: vendorId,
                    vendorMemberId: vendorMemberId,
                    operatorId: operatorId,
                    firstName: firstName,
                    lastName: lastName,
                    oddsType: oddsType,
                    currency: currency,
                    maxTransfer: maxTransfer,
                    minTransfer: minTransfer)
                .AsTask())
            .ConfigureAwait(false);
    }
}
