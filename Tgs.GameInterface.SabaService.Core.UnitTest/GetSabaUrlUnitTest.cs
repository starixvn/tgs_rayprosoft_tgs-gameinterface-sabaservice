using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SabaService.Core.Exceptions;

namespace Tgs.GameInterface.SabaService.Core.UnitTests;

public class GetSabaUrlUnitTest
{
	[Fact]
	public async Task Player_Does_Not_Exist()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";
		var platformType = GamePlatformType.Desktop;

		var url = new Uri(apiUrl, "/api/GetSabaUrl/");
		var response = "{\"error_code\":2,\"message\":\"member not found\"}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut.GetSabaUrlAsync(
			apiUrl: apiUrl,
			vendorId: vendorId,
			vendorMemberId: vendorMemberId,
			platformType: platformType);

		Assert.NotNull(actual);
		Assert.False(actual.IsPlayerExist);
	}

	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";
		var platformType = GamePlatformType.Desktop;

		var url = new Uri(apiUrl, "/api/GetSabaUrl/");
		var response = "{\"error_code\":0,\"message\":\"\",\"Data\":\"https://h0i2to.pg5688.com/DepositProcessLogin?token=nbHtzSPjpkmjl6n0d6C51w\"}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut.GetSabaUrlAsync(
			apiUrl: apiUrl,
			vendorId: vendorId,
			vendorMemberId: vendorMemberId,
			platformType: platformType);

		Assert.NotNull(actual);
		Assert.True(actual.IsPlayerExist);
		Assert.False(string.IsNullOrWhiteSpace(actual.GameUrl));
	}

	[Fact]
	public async Task Other_Errors()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";
		var platformType = GamePlatformType.Desktop;

		var url = new Uri(apiUrl, "/api/GetSabaUrl/");
		var response = "{\"error_code\":1,\"message\":\"Failed during executed\",\"Data\":null}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		_ = await Assert.ThrowsAsync<GameProviderResponseErrorException>(() =>
				sut.GetSabaUrlAsync(
					apiUrl: apiUrl,
					vendorId: vendorId,
					vendorMemberId: vendorMemberId,
					platformType: platformType)
				.AsTask());
	}
}
