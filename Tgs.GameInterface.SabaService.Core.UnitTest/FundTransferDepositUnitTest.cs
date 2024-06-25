using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SabaService.Core.Exceptions;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

public class FundTransferDepositUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";
		var vendorTransId = $"tg_{Guid.NewGuid():N}";
		var directionType = GameDirectionType.Deposit;
		var walletType = GameWalletType.Sportsbook;
		var currency = 20;
		var amount = 1m;

		var url = new Uri(apiUrl, "/api/FundTransfer/");
		var response = "{\"error_code\":0,\"message\":\"Success\",\"Data\":{\"trans_id\":3120,\"before_amount\":0.0,\"after_amount\":1.0,\"bonus_before_amount\":0,\"bonus_after_amount\":0.0,\"system_id\":\"ZF055PMWUUS19001\",\"status\":0}}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut.FundTransferAsync(
			apiUrl: apiUrl,
			vendorId: vendorId,
			vendorMemberId: vendorMemberId,
			vendorTransId: vendorTransId,
			directionType: directionType,
			walletType: walletType,
			currency: currency,
			amount: amount);

		Assert.NotNull(actual);
		Assert.True(actual.IsOrderExist);
		Assert.Equal(1m, actual.AfterAmount);
	}

	[Fact]
	public async Task Bonus_before_amount_is_null()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";
		var vendorTransId = $"tg_{Guid.NewGuid():N}";
		var directionType = GameDirectionType.Deposit;
		var walletType = GameWalletType.Sportsbook;
		var currency = 20;
		var amount = 1m;

		var url = new Uri(apiUrl, "/api/FundTransfer/");
		var response = "{\"error_code\":0,\"message\":\"Success\",\"Data\":{\"trans_id\":43,\"before_amount\":987.5,\"after_amount\":0.5,\"system_id\":\"PegaINH03S002\",\"status\":0}}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut.FundTransferAsync(
			apiUrl: apiUrl,
			vendorId: vendorId,
			vendorMemberId: vendorMemberId,
			vendorTransId: vendorTransId,
			directionType: directionType,
			walletType: walletType,
			currency: currency,
			amount: amount);

		Assert.NotNull(actual);
		Assert.True(actual.IsOrderExist);
		Assert.Equal(987.5m, actual.BeforeAmount);
		Assert.Equal(0.5m, actual.AfterAmount);
		Assert.Equal(0.0m, actual.BonusBeforeAmount);
		Assert.Equal(0.0m, actual.BonusAfterAmount);
	}

	[Fact]
	public async Task Pending()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";
		var vendorTransId = $"tg_{Guid.NewGuid():N}";
		var directionType = GameDirectionType.Deposit;
		var walletType = GameWalletType.Sportsbook;
		var currency = 20;
		var amount = 1m;

		var url = new Uri(apiUrl, "/api/FundTransfer/");
		var response = "{\"error_code\":1,\"message\":\"Pending\",\"Data\":{\"status\":2}}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut.FundTransferAsync(
			apiUrl: apiUrl,
			vendorId: vendorId,
			vendorMemberId: vendorMemberId,
			vendorTransId: vendorTransId,
			directionType: directionType,
			walletType: walletType,
			currency: currency,
			amount: amount);

		Assert.NotNull(actual);
		Assert.True(actual.Check);
		Assert.False(actual.IsOrderExist);
	}

	[Fact]
	public async Task Other_Errors()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";
		var vendorTransId = $"tg_{Guid.NewGuid():N}";
		var directionType = GameDirectionType.Deposit;
		var walletType = GameWalletType.Sportsbook;
		var currency = 20;
		var amount = 1m;

		var url = new Uri(apiUrl, "/api/FundTransfer/");
		var response = "{\"error_code\":2,\"message\":\"User does not exist.\",\"Data\":{\"status\":1}}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);
		_ = await Assert.ThrowsAsync<GameProviderResponseErrorException>(() =>
				sut.FundTransferAsync(
					apiUrl: apiUrl,
					vendorId: vendorId,
					vendorMemberId: vendorMemberId,
					vendorTransId: vendorTransId,
					directionType: directionType,
					walletType: walletType,
					currency: currency,
					amount: amount)
				.AsTask());
	}
}
