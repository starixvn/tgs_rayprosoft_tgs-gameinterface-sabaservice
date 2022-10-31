using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class PlayerBalanceGetServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();
        var fakeVendorMember = Substitute.For<IVendorMember>();

        var query = new PlayerBalanceQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VendorMemberId: "4ucmqplxb029");

        var vendorMemberId = $"{query.OperatorId}_{query.VendorMemberId}_test";
        _ = fakeVendorMember
            .ConvertToGameProviderMember(
                operatorId: Arg.Is(query.OperatorId),
                vendorMemberId: Arg.Is(query.VendorMemberId))
            .Returns(vendorMemberId);

        var result = new GetBalanceResult(
            IsPlayerExist: true,
            Balance: 100);
        _ = fakeGameApiClient
            .CheckUserBalanceAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                vendorMemberId: Arg.Is(vendorMemberId),
                walletType: Arg.Is(GameWalletType.Sportsbook))
            .Returns(result);

        var sut = new PlayerBalanceGetService(
            fakeGameApiClient,
            fakeVendorMember);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.IsPlayerExist);
    }
}
