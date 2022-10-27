using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class GetBetDetailUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var versionKey = 638356;

        var url = new Uri(apiUrl, "/api/GetBetDetail/");
        var response = "{\"error_code\":0,\"message\":\"\",\"Data\":{\"last_version_key\":645302,\"BetDetails\":[{\"trans_id\":43878244893589518,\"vendor_member_id\":\"tg_4ucmqplxb029_test\",\"operator_id\":\"tg\",\"league_id\":104382,\"leaguename\":[{\"lang\":\"en\",\"name\":\"CHINA LEAGUE 2 (PLAY OFF)\"},{\"lang\":\"cs\",\"name\":\"\\u4E2D\\u56FD\\u4E59\\u7EC4\\u8054\\u8D5B (\\u9644\\u52A0\\u8D5B)\"}],\"match_id\":59616197,\"home_id\":749486,\"hometeamname\":[{\"lang\":\"en\",\"name\":\"Dongguan Guanlian\"},{\"lang\":\"cs\",\"name\":\"\\u4E1C\\u839E\\u839E\\u8054\"}],\"away_id\":749497,\"awayteamname\":[{\"lang\":\"en\",\"name\":\"Yanbian Longding\"},{\"lang\":\"cs\",\"name\":\"\\u5EF6\\u8FB9\\u9F99\\u9F0E\"}],\"match_datetime\":\"2022-10-27T02:00:00\",\"sport_type\":1,\"sportname\":[{\"lang\":\"en\",\"name\":\"Soccer\"},{\"lang\":\"cs\",\"name\":\"\\u8DB3\\u7403\"}],\"bet_type\":1,\"bettypename\":[{\"lang\":\"en\",\"name\":\"Handicap\"},{\"lang\":\"cs\",\"name\":\"\\u8BA9\\u7403\"}],\"parlay_ref_no\":0,\"odds\":0.9800,\"stake\":1.0000,\"transaction_time\":\"2022-10-27T01:50:00.643\",\"ticket_status\":\"running\",\"winlost_amount\":-1.0000,\"after_amount\":1.0000,\"currency\":20,\"winlost_datetime\":\"2022-10-27T00:00:00\",\"odds_type\":2,\"bet_team\":\"h\",\"isLucky\":\"False\",\"home_hdp\":0.0000,\"away_hdp\":0.2500,\"hdp\":0.25,\"betfrom\":\"x\",\"islive\":\"0\",\"home_score\":null,\"away_score\":null,\"settlement_time\":null,\"customInfo1\":\"\",\"customInfo2\":\"\",\"customInfo3\":\"\",\"customInfo4\":\"\",\"customInfo5\":\"\",\"ba_status\":\"0\",\"version_key\":645302,\"ParlayData\":null}]}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetBetDetailAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                versionKey: versionKey)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(645302 == actual.LastVersionKey);
        Assert.IsTrue(actual.BetDetails.Any());
    }

    [TestMethod]
    public async Task No_Data()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var versionKey = 645564;

        var url = new Uri(apiUrl, "/api/GetBetDetail/");
        var response = "{\"error_code\":0,\"message\":\"\",\"Data\":{\"last_version_key\":645564}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetBetDetailAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                versionKey: versionKey)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(645564 == actual.LastVersionKey);
        Assert.IsFalse(actual.BetDetails.Any());
    }
}
