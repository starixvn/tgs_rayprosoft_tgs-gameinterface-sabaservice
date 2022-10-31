using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SabaService.Core.Exceptions;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class CreateMemberUnitTest
{
    [TestMethod]
    public async Task Player_Already_Exists()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorMemberId = "tg_4ucmqplxb029_test";
        string operatorId = "tg";
        string firstName = "GPI";
        string lastName = "4ucmqplxb029";
        int oddsType = 2;
        int currency = 13;
        decimal maxTransfer = 9999999999;
        decimal minTransfer = 0;

        var url = new Uri(apiUrl, "/api/CreateMember/");
        var response = "{\"error_code\":6,\"message\":\"Duplicate Vendor_Member_ID \"}";
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
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorMemberId = "tg_4ucmqplxb029_test";
        string operatorId = "tg";
        string firstName = "GPI";
        string lastName = "4ucmqplxb029";
        int oddsType = 2;
        int currency = 20;
        decimal maxTransfer = 9999999999;
        decimal minTransfer = 0;

        var url = new Uri(apiUrl, "/api/CreateMember/");
        var response = "{\"error_code\":0,\"message\":\"\"}";
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
    public async Task Other_Errors()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorMemberId = "tg_4ucmqplxb029_test";
        string operatorId = "tg";
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
