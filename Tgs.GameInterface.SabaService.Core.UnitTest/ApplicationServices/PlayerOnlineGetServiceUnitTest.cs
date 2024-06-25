using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

public class PlayerOnlineGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

		var query = new PlayerOnlineQuery(
			ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
			OperatorId: "tg",
			VendorId: "um5vspnzkx",
			PlayerCode: "4ucmqplxb029");

		var vendorMemberId = $"{query.OperatorId}_{query.PlayerCode}_test";
		_ = fakeGameVendorMember
			.ConvertToGameProviderMember(
			   Arg.Is(query.OperatorId),
			   Arg.Is(query.PlayerCode))
			.Returns(vendorMemberId);

		_ = fakeGameApiClient.CheckIsOnlineAsync(
			apiUrl: Arg.Is(query.ApiUrl),
			vendorId: Arg.Is(query.VendorId),
			vendorMemberId: Arg.Is(vendorMemberId))
			.Returns(true);

		var sut = new PlayerOnlineGetService(
			fakeGameApiClient,
			fakeGameVendorMember);

		var actual = await sut.GetAsync(query);

		Assert.True(actual);
	}
}
