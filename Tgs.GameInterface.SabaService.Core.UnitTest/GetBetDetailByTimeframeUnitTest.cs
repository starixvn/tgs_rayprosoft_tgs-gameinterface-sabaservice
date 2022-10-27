using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using RichardSzalay.MockHttp;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class GetBetDetailByTimeframeUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var timeType = GameTimeType.Bet;
        var startDate = DateTime.Parse("2022-10-27T13:50:00+08:00");
        var endDate = DateTime.Parse("2022-10-27T13:51:00+08:00");

        var url = new Uri(apiUrl, "/api/GetBetDetailByTimeframe/");
        var response = "{\"error_code\":0,\"message\":\"\",\"Data\":{\"last_version_key\":0,\"BetDetails\":[{\"trans_id\":43878244893589518,\"vendor_member_id\":\"tg_4ucmqplxb029_test\",\"operator_id\":\"tg\",\"league_id\":104382,\"leaguename\":[{\"lang\":\"en\",\"name\":\"CHINA LEAGUE 2 (PLAY OFF)\"},{\"lang\":\"cs\",\"name\":\"中国乙组联赛 (附加赛)\"}],\"match_id\":59616197,\"home_id\":749486,\"hometeamname\":[{\"lang\":\"en\",\"name\":\"Dongguan Guanlian\"},{\"lang\":\"cs\",\"name\":\"东莞莞联\"}],\"away_id\":749497,\"awayteamname\":[{\"lang\":\"en\",\"name\":\"Yanbian Longding\"},{\"lang\":\"cs\",\"name\":\"延边龙鼎\"}],\"match_datetime\":\"2022-10-27T02:00:00\",\"sport_type\":1,\"sportname\":[{\"lang\":\"en\",\"name\":\"Soccer\"},{\"lang\":\"cs\",\"name\":\"足球\"}],\"bet_type\":1,\"bettypename\":[{\"lang\":\"en\",\"name\":\"Handicap\"},{\"lang\":\"cs\",\"name\":\"让球\"}],\"parlay_ref_no\":0,\"odds\":0.98,\"stake\":1.0,\"transaction_time\":\"2022-10-27T01:50:00.643\",\"ticket_status\":\"lose\",\"winlost_amount\":-1.0,\"after_amount\":1.0,\"currency\":20,\"winlost_datetime\":\"2022-10-27T00:00:00\",\"odds_type\":2,\"bet_team\":\"h\",\"isLucky\":\"False\",\"home_hdp\":0.0,\"away_hdp\":0.25,\"hdp\":0.25,\"betfrom\":\"x\",\"islive\":\"0\",\"home_score\":null,\"away_score\":null,\"settlement_time\":\"2022-10-27T03:52:22.907\",\"customInfo1\":\"\",\"customInfo2\":\"\",\"customInfo3\":\"\",\"customInfo4\":\"\",\"customInfo5\":\"\",\"ba_status\":\"0\",\"version_key\":645616,\"ParlayData\":null}]}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetBetDetailByTimeframeAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                timeType: timeType,
                startTime: startDate,
                endTime: endDate)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Any());
    }

    [TestMethod]
    public async Task No_Data()
    {
        var apiUrl = new Uri("http://h0i2tsa.bw6688.com/");
        var vendorId = "um5vspnzkx";
        var timeType = GameTimeType.Bet;
        var startDate = DateTime.Parse("2022-10-27T13:50:00+08:00");
        var endDate = DateTime.Parse("2022-10-27T13:51:00+08:00");

        var url = new Uri(apiUrl, "/api/GetBetDetailByTimeframe/");
        var response = "{\"error_code\":0,\"message\":\"\",\"Data\":{\"last_version_key\":0}}";
        var handler = new MockHttpMessageHandler();
        _ = handler.Expect(HttpMethod.Post, url.ToString())
                        .Respond(HttpStatusCode.OK, new StringContent(response));

        var fakeHttpClient = handler.ToHttpClient();
        var fakeLog = NullLogger<GameApiClient>.Instance;

        var sut = new GameApiClient(fakeHttpClient, fakeLog);

        var actual = await sut
            .GetBetDetailByTimeframeAsync(
                apiUrl: apiUrl,
                vendorId: vendorId,
                timeType: timeType,
                startTime: startDate,
                endTime: endDate)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsFalse(actual.Any());
    }
}
