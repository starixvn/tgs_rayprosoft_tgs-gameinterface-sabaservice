using System.Text.Json.Nodes;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;
using Xunit;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

public class FundInGetServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeGameApiClient = Substitute.For<IGameApiClient>();
		var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

		var query = new FundInQuery(
			ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
			OperatorId: "tg",
			VendorId: "um5vspnzkx",
			PlayerCode: "4ucmqplxb029",
			OrderId: Guid.NewGuid().ToString("N"),
			Currency: 20,
			Amount: 1m);

		var vendorMemberId = $"{query.OperatorId}_{query.PlayerCode}_test";
		_ = fakeGameVendorMember
			.ConvertToGameProviderMember(
			   Arg.Is(query.OperatorId),
			   Arg.Is(query.PlayerCode))
			.Returns(vendorMemberId);

		var vendorTransId = $"{query.OperatorId}_{query.OrderId}";

		var result = new FundTransferResult(
			Check: false,
			IsOrderExist: true,
			TransId: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
			Resp: "{\"trans_id\":3121,\"before_amount\":1.0,\"after_amount\":0.0,\"bonus_before_amount\":0,\"bonus_after_amount\":0.0,\"system_id\":\"ZF055PMWUUS19001\",\"status\":0}");
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
			fakeGameVendorMember);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.True(actual.IsOrderExist);
		Assert.False(actual.Check);
		Assert.Equal(query.OrderId, actual.OrderId);
		Assert.Equal(vendorTransId, actual.VendorTransId);
	}
}
