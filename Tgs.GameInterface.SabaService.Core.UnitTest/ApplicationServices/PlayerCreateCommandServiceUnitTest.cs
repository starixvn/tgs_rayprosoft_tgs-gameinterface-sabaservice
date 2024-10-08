using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;

namespace Tgs.GameInterface.SabaService.Core.UnitTests.ApplicationServices;

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

		var vendorMemberId = $"{command.BrandCode}_{command.PlayerCode}_test";
		_ = fakeGameVendorMember
			.ConvertToGameProviderMember(
			   Arg.Is(command.BrandCode),
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
			   oddsType: Arg.Is(command.OddsType),
			   currency: Arg.Is(command.Currency));
	}
}
