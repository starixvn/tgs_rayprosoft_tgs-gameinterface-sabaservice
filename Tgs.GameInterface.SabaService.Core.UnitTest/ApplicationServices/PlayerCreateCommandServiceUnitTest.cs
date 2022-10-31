using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class PlayerCreateCommandServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();
        var fakeVendorMember = Substitute.For<IVendorMember>();

        var command = new PlayerCreateCommand(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            BrandCode: "GPI",
            VendorMemberId: "4ucmqplxb029",
            OddsType: 2,
            Currency: 20);

        var vendorMemberId = $"{command.OperatorId}_{command.VendorMemberId}_test";
        _ = fakeVendorMember
            .ConvertToGameProviderMember(
                operatorId: Arg.Is(command.OperatorId),
                vendorMemberId: Arg.Is(command.VendorMemberId))
            .Returns(vendorMemberId);

        var sut = new PlayerCreateCommandService(
            fakeGameApiClient,
            fakeVendorMember);

        await sut
           .ExecuteAsync(command)
           .ConfigureAwait(false);

        await fakeGameApiClient
           .Received(1)
           .CreateMemberAsync(
               apiUrl: Arg.Is(command.ApiUrl),
               operatorId: Arg.Is(command.OperatorId),
               vendorId: Arg.Is(command.VendorId),
               vendorMemberId: vendorMemberId,
               firstName: Arg.Is(command.BrandCode),
               lastName: Arg.Is(command.VendorMemberId),
               oddsType: Arg.Is(command.OddsType),
               currency: Arg.Is(command.Currency))
           .ConfigureAwait(false);
    }
}
