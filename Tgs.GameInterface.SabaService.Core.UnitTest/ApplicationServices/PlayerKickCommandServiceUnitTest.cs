using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class PlayerKickCommandServiceUnitTest
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

        var command = new PlayerKickCommand(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VendorMemberId: "4ucmqplxb029");

        var vendorMemberId = $"{command.OperatorId}_{command.VendorMemberId}_test";

        _ = fakeGameApiClient
            .KickUserAsync(
                apiUrl: Arg.Is(command.ApiUrl),
                vendorId: Arg.Is(command.VendorId),
                vendorMemberId: Arg.Is(vendorMemberId));

        var sut = new PlayerKickCommandService(
            fakeGameApiClient,
            vendorMember);

        await sut
         .ExecuteAsync(command)
         .ConfigureAwait(false);
    }
}
