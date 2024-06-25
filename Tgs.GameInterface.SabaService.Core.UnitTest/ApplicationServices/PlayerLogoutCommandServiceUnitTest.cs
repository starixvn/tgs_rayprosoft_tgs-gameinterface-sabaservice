using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

public class PlayerLogoutCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

		var command = new PlayerLogoutCommand(
			ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
			OperatorId: "tg",
			VendorId: "um5vspnzkx",
			PlayerCode: "4ucmqplxb029");

		var vendorMemberId = $"{command.OperatorId}_{command.PlayerCode}_test";
		_ = fakeGameVendorMember
			.ConvertToGameProviderMember(
			   Arg.Is(command.OperatorId),
			   Arg.Is(command.PlayerCode))
			.Returns(vendorMemberId);

		_ = fakeGameApiClient
			.KickUserAsync(
				apiUrl: Arg.Is(command.ApiUrl),
				vendorId: Arg.Is(command.VendorId),
				vendorMemberId: Arg.Is(vendorMemberId));

		var sut = new PlayerLogoutCommandService(
			fakeGameApiClient,
			fakeGameVendorMember);

		await sut.ExecuteAsync(command);
	}
}
