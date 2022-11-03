using System.Text.Json;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class GameHistoryGetServiceUnitTest
{
    [TestMethod]
    public async Task Waiting()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"Waiting",
                    TicketExtraStatus:null,
                    Stake:10,
                    WinlostAmount:0,
                    BuybackAmount:null,
                    TransactionTime:DateTime.Now,
                    SettlementTime:DateTime.UnixEpoch,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" }))
            });
        _ = fakeGameApiClient
            .GetBetDetailAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                versionKey: Arg.Is(query.VersionKey))
            .Returns(result);

        var sut = new GameHistoryGetService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsFalse(actual.Details.Any());
    }

    [TestMethod]
    public async Task Settled()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"Won",
                    TicketExtraStatus:null,
                    Stake:10,
                    WinlostAmount:10,
                    BuybackAmount:null,
                    TransactionTime:DateTime.UtcNow.AddHours(-1),
                    SettlementTime:DateTime.UtcNow,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" }))
            });
        _ = fakeGameApiClient
            .GetBetDetailAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                versionKey: Arg.Is(query.VersionKey))
            .Returns(result);

        var sut = new GameHistoryGetService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
        Assert.IsFalse(actual.Details.First().IsCashout);
        Assert.IsTrue(actual.Details.First().IsClosed);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.Details.First().PlayerCode));
        Assert.IsTrue(10m == actual.Details.First().ValidBet);
        Assert.IsTrue(10m == actual.Details.First().Turnover);
        Assert.IsTrue(20m == actual.Details.First().Settle);
        Assert.IsTrue(result.Details.First().TransactionTime == actual.Details.First().ExternalCreatedOn);
        Assert.IsTrue(result.Details.First().SettlementTime == actual.Details.First().ExternalClosedOn);
    }

    [TestMethod]
    public async Task Unsettled()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"Running",
                    TicketExtraStatus:null,
                    Stake:10,
                    WinlostAmount:0,
                    BuybackAmount:null,
                    TransactionTime:DateTime.UtcNow.AddHours(-1),
                    SettlementTime:DateTime.UnixEpoch,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" }))
            });
        _ = fakeGameApiClient
            .GetBetDetailAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                versionKey: Arg.Is(query.VersionKey))
            .Returns(result);

        var sut = new GameHistoryGetService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
        Assert.IsTrue("4ucmqplxb029".Equals(actual.Details.First().PlayerCode));
        Assert.IsTrue(result.Details.First().TransactionTime == actual.Details.First().ExternalCreatedOn);
        Assert.IsTrue(DateTime.UnixEpoch == actual.Details.First().ExternalClosedOn);
    }

    [TestMethod]
    public async Task Settled_SportType不是1到99範圍結算改使用WinlostDatetime()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:100,
                    TicketStatus:"Won",
                    TicketExtraStatus:null,
                    Stake:10,
                    WinlostAmount:0,
                    BuybackAmount:null,
                    TransactionTime:DateTime.UtcNow.AddHours(-1),
                    SettlementTime:DateTime.UtcNow,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" }))
            });
        _ = fakeGameApiClient
            .GetBetDetailAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                versionKey: Arg.Is(query.VersionKey))
            .Returns(result);

        var sut = new GameHistoryGetService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
        Assert.IsTrue("4ucmqplxb029".Equals(actual.Details.First().PlayerCode));
        Assert.IsTrue(result.Details.First().TransactionTime == actual.Details.First().ExternalCreatedOn);
        Assert.IsTrue(result.Details.First().WinlostDatetime == actual.Details.First().ExternalClosedOn);
    }

    [TestMethod]
    public async Task 取得資料同時有Waiting_Unsettled_Settled()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"Waiting",
                    TicketExtraStatus:null,
                    Stake:10,
                    WinlostAmount:0,
                    BuybackAmount:null,
                    TransactionTime:DateTime.Now,
                    SettlementTime:DateTime.UnixEpoch,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" })),
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"Running",
                    TicketExtraStatus:null,
                    Stake:10,
                    WinlostAmount:0,
                    BuybackAmount:null,
                    TransactionTime:DateTime.UtcNow.AddHours(-1),
                    SettlementTime:DateTime.UnixEpoch,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" })),
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"Won",
                    TicketExtraStatus:null,
                    Stake:10,
                    WinlostAmount:10,
                    BuybackAmount:null,
                    TransactionTime:DateTime.UtcNow.AddHours(-1),
                    SettlementTime:DateTime.UtcNow,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" }))
            });
        _ = fakeGameApiClient
            .GetBetDetailAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                    vendorId: Arg.Is(query.VendorId),
                    versionKey: Arg.Is(query.VersionKey))
                .Returns(result);

        var sut = new GameHistoryGetService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
        Assert.IsTrue(actual.Details.Count() == 2);
    }

    [TestMethod]
    public async Task Cashout()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"Refund",
                    TicketExtraStatus:"cashout",
                    Stake:10,
                    WinlostAmount:0,
                    BuybackAmount:10m,
                    TransactionTime:DateTime.UtcNow.AddHours(-1),
                    SettlementTime:DateTime.UtcNow,
                    WinlostDatetime:DateTime.Today,
                    AdditionalData:JsonSerializer.Serialize(new{VendorMemberId="tg_4ucmqplxb029_test" }))
            });
        _ = fakeGameApiClient
            .GetBetDetailAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                versionKey: Arg.Is(query.VersionKey))
            .Returns(result);

        var sut = new GameHistoryGetService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
        Assert.IsTrue(actual.Details.First().IsCashout);
        Assert.IsTrue(actual.Details.First().IsClosed);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.Details.First().PlayerCode));
        Assert.IsTrue(0m == actual.Details.First().ValidBet);
        Assert.IsTrue(0m == actual.Details.First().Turnover);
        Assert.IsTrue(10m == actual.Details.First().Settle);
        Assert.IsTrue(result.Details.First().TransactionTime == actual.Details.First().ExternalCreatedOn);
        Assert.IsTrue(result.Details.First().SettlementTime == actual.Details.First().ExternalClosedOn);
    }

    [TestMethod]
    public async Task Reject()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new BetDetail(
                    TransId:DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    VendorMemberId:"tg_4ucmqplxb029_test",
                    SportType:1,
                    TicketStatus:"reject",
                    TicketExtraStatus:null,
                    Stake:1,
                    WinlostAmount:0,
                    BuybackAmount:null,
                    TransactionTime:DateTime.Parse("2022-11-02T21:15:18.44-04:00"),
                    SettlementTime:DateTime.UnixEpoch,
                    WinlostDatetime:DateTime.Parse("2022-11-02T00:00:00-04:00"),
                    AdditionalData:"{\"trans_id\":46405051463237660,\"vendor_member_id\":\"tgidr_4uguvssjj02s_test\",\"operator_id\":\"tgidr\",\"league_id\":4679,\"leaguename\":[{\"lang\":\"en\",\"name\":\"COLOMBIA CUP\"},{\"lang\":\"cs\",\"name\":\"哥伦比亚杯\"}],\"match_id\":59853308,\"home_id\":9062,\"hometeamname\":[{\"lang\":\"en\",\"name\":\"Millonarios\"},{\"lang\":\"cs\",\"name\":\"米伦拿列奥\"}],\"away_id\":32425,\"awayteamname\":[{\"lang\":\"en\",\"name\":\"Junior Barranquilla\"},{\"lang\":\"cs\",\"name\":\"巴兰基利亚青年\"}],\"match_datetime\":\"2022-11-02T21:00:00\",\"sport_type\":1,\"sportname\":[{\"lang\":\"en\",\"name\":\"Soccer\"},{\"lang\":\"cs\",\"name\":\"足球\"}],\"bet_type\":15,\"bettypename\":[{\"lang\":\"en\",\"name\":\"1H 1X2\"},{\"lang\":\"cs\",\"name\":\"上半场.独赢盘\"}],\"parlay_ref_no\":0,\"odds\":1.78,\"stake\":1,\"transaction_time\":\"2022-11-02T21:15:18.44\",\"ticket_status\":\"reject\",\"winlost_amount\":0,\"after_amount\":97,\"currency\":20,\"winlost_datetime\":\"2022-11-02T00:00:00\",\"odds_type\":3,\"bet_team\":\"x\",\"isLucky\":\"False\",\"home_hdp\":1.1,\"away_hdp\":1,\"hdp\":null,\"betfrom\":\"x\",\"islive\":\"1\",\"home_score\":0,\"away_score\":0,\"settlement_time\":null,\"customInfo1\":\"\",\"customInfo2\":\"\",\"customInfo3\":\"\",\"customInfo4\":\"\",\"customInfo5\":\"\",\"ba_status\":\"0\",\"version_key\":670749,\"ParlayData\":null}"
                    )
            });
        _ = fakeGameApiClient
            .GetBetDetailAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                versionKey: Arg.Is(query.VersionKey))
            .Returns(result);

        var sut = new GameHistoryGetService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
        Assert.IsFalse(actual.Details.First().IsCashout);
        Assert.IsTrue(actual.Details.First().IsClosed);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.Details.First().PlayerCode));
        Assert.IsTrue(1m == actual.Details.First().Bet);
        Assert.IsTrue(0m == actual.Details.First().ValidBet);
        Assert.IsTrue(0m == actual.Details.First().Turnover);
        Assert.IsTrue(0m == actual.Details.First().Settle);
        Assert.IsTrue(result.Details.First().TransactionTime == actual.Details.First().ExternalCreatedOn);
        Assert.IsTrue(result.Details.First().WinlostDatetime == actual.Details.First().ExternalClosedOn);
    }
}
