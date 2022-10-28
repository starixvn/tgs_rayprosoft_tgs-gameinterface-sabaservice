using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class FundStatusGetServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var query = new FundStatusQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VendorTransId: Guid.NewGuid().ToString("N"));

        var vendorTransId = $"{query.OperatorId}_{query.VendorTransId}";

        var result = new CheckFundTransferResult(
                    Retry: false,
                    IsOrderExist: true);
        _ = fakeGameApiClient
            .CheckFundTransferAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                vendorTransId: Arg.Is(vendorTransId),
                walletType: Arg.Is(GameWalletType.Sportsbook))
            .Returns(result);

        var sut = new FundStatusGetService(
            fakeGameApiClient);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsOrderExist);
        Assert.IsFalse(actual.Retry);
    }
}
