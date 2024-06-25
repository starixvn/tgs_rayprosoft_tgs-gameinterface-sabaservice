using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

public class PlayerCreateCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

		var command = new PlayerCreateCommand(
			ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
			OperatorId: "tg",
			VendorId: "um5vspnzkx",
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			OddsType: 2,
			Currency: 20);

		var vendorMemberId = $"{command.OperatorId}_{command.PlayerCode}_test";
		_ = fakeGameVendorMember
			.ConvertToGameProviderMember(
			   Arg.Is(command.OperatorId),
			   Arg.Is(command.PlayerCode))
			.Returns(vendorMemberId);

		var sut = new PlayerCreateCommandService(
			fakeGameApiClient,
			fakeGameVendorMember);

		await sut.ExecuteAsync(command);

		await fakeGameApiClient
		   .Received(1)
		   .CreateMemberAsync(
			   apiUrl: Arg.Is(command.ApiUrl),
			   operatorId: Arg.Is(command.OperatorId),
			   vendorId: Arg.Is(command.VendorId),
			   vendorMemberId: vendorMemberId,
			   firstName: Arg.Is(command.BrandCode),
			   lastName: Arg.Is(command.PlayerCode),
			   oddsType: Arg.Is(command.OddsType),
			   currency: Arg.Is(command.Currency));
	}
}
