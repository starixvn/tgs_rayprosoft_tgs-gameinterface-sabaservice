using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SabaService.Core.Exceptions;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class CheckFundTransferUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorTransId = "tg_a7185c474cf3421495d6be38130dddc3";
        var walletType = GameWalletType.Sportsbook;

        var url = new Uri(apiUrl, "/api/CheckFundTransfer/");
        var response = "{\"error_code\":0,\"message\":\"Check is successful, Success\",\"Data\":{\"trans_id\":3126,\"transfer_date\":\"2022-10-26T23:15:26\",\"vender_member_id\":\"tg_4ucmqplxb029_test\",\"amount\":1.00,\"currency\":20,\"before_amount\":4.0000,\"after_amount\":5.0,\"status\":0}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .CheckFundTransferAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorTransId: vendorTransId,
                walletType: walletType)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsOrderExist);
    }

    [TestMethod]
    public async Task Pending()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorTransId = "tg_57ef2e096fbc4e7683747e6f56b822b3";
        var walletType = GameWalletType.Sportsbook;

        var url = new Uri(apiUrl, "/api/CheckFundTransfer/");
        var response = "{\"error_code\":3,\"message\":\"Please wait 4 minutes, and then to check again.\",\"Data\":{\"trans_id\":3128,\"transfer_date\":\"2022-10-26T23:36:19.257\",\"vender_member_id\":\"tg_4ucmqplxb029_test\",\"amount\":1.0,\"currency\":20,\"before_amount\":6.0,\"after_amount\":0.0,\"status\":2}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .CheckFundTransferAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorTransId: vendorTransId,
                walletType: walletType)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Retry);
        Assert.IsFalse(actual.IsOrderExist);
    }

    [TestMethod]
    public async Task Failed()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorTransId = "tg_57ef2e096fbc4e7683747e6f56b822b3";
        var walletType = GameWalletType.Sportsbook;

        var url = new Uri(apiUrl, "/api/CheckFundTransfer/");
        var response = "{\"error_code\":0,\"message\":\"Check is successful, Check is successful, the transaction transfers fail.\",\"Data\":{\"trans_id\":3128,\"transfer_date\":\"2022-10-26T23:36:19.257\",\"vender_member_id\":\"tg_4ucmqplxb029_test\",\"amount\":1.0,\"currency\":20,\"before_amount\":6.0,\"after_amount\":6.0,\"status\":1}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .CheckFundTransferAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                vendorTransId: vendorTransId,
                walletType: walletType)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsFalse(actual.Retry);
        Assert.IsFalse(actual.IsOrderExist);
    }

    [TestMethod]
    public async Task Other_Errors()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var vendorTransId = "tg_23fc3bc39dc64bb89b21f17a00dd80c9";
        var walletType = GameWalletType.Sportsbook;

        var url = new Uri(apiUrl, "/api/CheckFundTransfer/");
        var response = "{\"error_code\":2,\"message\":\"Transaction record does not exist\"}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);
        _ = await Assert
            .ThrowsExceptionAsync<GameProviderResponseErrorException>(() =>
                sut
                .CheckFundTransferAsync(
                    apiUrl: apiUrl,
                    vendorId: vendorId,
                    vendorTransId: vendorTransId,
                    walletType: walletType)
                .AsTask())
            .ConfigureAwait(false);
    }
}
