using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class FundInGetServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var query = new FundInQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VendorMemberId: "4ucmqplxb029",
            VendorTransId: Guid.NewGuid().ToString("N"),
            Currency: 20,
            Amount: 1m);

        var vendorMemberId = $"{query.OperatorId}_{query.VendorMemberId}_test";

        var vendorTransId = $"{query.OperatorId}_{query.VendorTransId}";

        var result = new FundTransferResult(
            Check: false,
            IsOrderExist: true,
            TransId: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            AfterAmount: 0,
            BeforeAmount: 1,
            BonusAfterAmount: 0,
            BonusBeforeAmount: 0,
            SystemId: "ZF055PMWUUS19001");
        _ = fakeGameApiClient
            .FundTransferAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                vendorMemberId: Arg.Is(vendorMemberId),
                vendorTransId: Arg.Is(vendorTransId),
                directionType: Arg.Is(GameDirectionType.Withdraw),
                walletType: Arg.Is(GameWalletType.Sportsbook),
                currency: query.Currency,
                amount: query.Amount)
            .Returns(result);

        var sut = new FundInGetService(
            fakeGameApiClient,
            vendorMember);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsOrderExist);
        Assert.IsFalse(actual.Check);
        Assert.AreEqual(vendorTransId, actual.ActualVendorTransId);
        Assert.AreEqual(result.AfterAmount, actual.Balance);
    }
}
