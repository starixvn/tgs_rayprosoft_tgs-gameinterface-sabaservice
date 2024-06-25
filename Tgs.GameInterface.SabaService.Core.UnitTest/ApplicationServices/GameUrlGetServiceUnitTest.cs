using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

public class GameUrlGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

		var query = new GameUrlQuery(
			ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
			OperatorId: "tg",
			VendorId: "um5vspnzkx",
			PlayerCode: "4ucmqplxb029",
			IsMobile: false);

		var vendorMemberId = $"{query.OperatorId}_{query.PlayerCode}_test";
		_ = fakeGameVendorMember
			.ConvertToGameProviderMember(
			   Arg.Is(query.OperatorId),
			   Arg.Is(query.PlayerCode))
			.Returns(vendorMemberId);

		var result = new GetGameUrlResult(
			IsPlayerExist: true,
			GameUrl: "http://h0i2ob11.SABADomain.com/Deposit_ProcessLogin.aspx?token=xxxxxxx&lang=xx");
		_ = fakeGameApiClient
			.GetSabaUrlAsync(
				apiUrl: Arg.Is(query.ApiUrl),
				vendorId: Arg.Is(query.VendorId),
				vendorMemberId: Arg.Is(vendorMemberId),
				platformType: Arg.Is(GamePlatformType.Desktop))
			.Returns(result);

		var sut = new GameUrlGetService(
			fakeGameApiClient,
			fakeGameVendorMember);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.Equal(actual.GameUrl, result.GameUrl);
	}
}
