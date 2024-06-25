using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;
using Tgs.GameInterface.SabaService.Core.Exceptions;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

public class CheckIsOnlineUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
		var vendorId = "um5vspnzkx";
		var vendorMemberId = "tg_4ucmqplxb029_test";

		var url = new Uri(apiUrl, "/api/CheckIsOnline/");
		var response = "{\"error_code\":0,\"message\":\"\",\"Data\":[{\"vendor_member_id\":\"4ucmqplxb029\",\"is_online\":true,\"error_code\":0}]}";
		var handler = new MockHttpMessageHandler();
		_ = handler.Expect(HttpMethod.Post, url.ToString())
						.Respond(HttpStatusCode.OK, new StringContent(response));

		var fakeHttpClient = handler.ToHttpClient();
		var fakeLog = NullLogger<GameApiClient>.Instance;

		var sut = new GameApiClient(fakeHttpClient, fakeLog);

		var actual = await sut.CheckIsOnlineAsync(
			apiUrl: apiUrl,
			vendorId: vendorId,
			vendorMemberId: vendorMemberId);

		Assert.True(actual);
	}
}
