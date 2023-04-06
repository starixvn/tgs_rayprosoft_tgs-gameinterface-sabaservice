using System.Text.Json;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest;

[TestClass]
public class GameHistoryCalculateUnitTest
{
    [TestMethod]
    public void Settled()
    {
        var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

        var operatorId = "tg";
        var detail = new BetDetail(
            TransId: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VendorMemberId: "tg_4ucmqplxb029_test",
            SportType: 1,
            TicketStatus: "Won",
            TicketExtraStatus: null,
            Stake: 10,
            WinlostAmount: 10,
            BuybackAmount: null,
            TransactionTime: DateTime.UtcNow.AddHours(-1),
            SettlementTime: DateTime.UtcNow,
            WinlostDatetime: DateTime.Today,
            AdditionalData: JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" }));

        _ = fakeGameVendorMember
            .ConvertToPlatformMember(
                Arg.Is(operatorId),
                Arg.Is(detail.VendorMemberId))
            .Returns("4ucmqplxb029");

        var gameHistoryCalculate = new GameHistoryCalculate(fakeGameVendorMember);

        var actual = gameHistoryCalculate
            .Execute(
                operatorId,
                detail);

        Assert.IsNotNull(actual);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.PlayerCode));
        Assert.IsTrue(actual.IsClosed);
        Assert.IsTrue(10m == actual.Bet);
        Assert.IsTrue(10m == actual.ValidBet);
        Assert.IsTrue(20m == actual.Settle);
        Assert.IsTrue(10m == actual.Turnover);
        Assert.IsTrue(10m == actual.PlayerWinLoss);
        Assert.IsTrue(detail.TransactionTime == actual.ExternalCreatedOn);
        Assert.IsTrue(detail.SettlementTime == actual.ExternalClosedOn);
    }

    [TestMethod]
    public void Unsettled()
    {
        var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

        var operatorId = "tg";
        var detail = new BetDetail(
            TransId: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VendorMemberId: "tg_4ucmqplxb029_test",
            SportType: 1,
            TicketStatus: "Running",
            TicketExtraStatus: null,
            Stake: 10,
            WinlostAmount: 0,
            BuybackAmount: null,
            TransactionTime: DateTime.UtcNow.AddHours(-1),
            SettlementTime: DateTime.UnixEpoch,
            WinlostDatetime: DateTime.Today,
            AdditionalData: JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" }));

        _ = fakeGameVendorMember
            .ConvertToPlatformMember(
                Arg.Is(operatorId),
                Arg.Is(detail.VendorMemberId))
            .Returns("4ucmqplxb029");

        var gameHistoryCalculate = new GameHistoryCalculate(fakeGameVendorMember);

        var actual = gameHistoryCalculate
            .Execute(
                operatorId,
                detail);

        Assert.IsNotNull(actual);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.PlayerCode));
        Assert.IsFalse(actual.IsClosed);
        Assert.IsTrue(10m == actual.ValidBet);
        Assert.IsTrue(0m == actual.Turnover);
        Assert.IsTrue(0m == actual.Settle);
        Assert.IsTrue(detail.TransactionTime == actual.ExternalCreatedOn);
        Assert.IsTrue(DateTime.UnixEpoch == actual.ExternalClosedOn);
    }

    [TestMethod]
    public void Settled_SportType不是1到99範圍結算改使用WinlostDatetime()
    {
        var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

        var operatorId = "tg";
        var detail = new BetDetail(
            TransId: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VendorMemberId: "tg_4ucmqplxb029_test",
            SportType: 100,
            TicketStatus: "Won",
            TicketExtraStatus: null,
            Stake: 10,
            WinlostAmount: 10,
            BuybackAmount: null,
            TransactionTime: DateTime.UtcNow.AddHours(-1),
            SettlementTime: DateTime.UtcNow,
            WinlostDatetime: DateTime.Today,
            AdditionalData: JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" }));

        _ = fakeGameVendorMember
            .ConvertToPlatformMember(
                Arg.Is(operatorId),
                Arg.Is(detail.VendorMemberId))
            .Returns("4ucmqplxb029");

        var gameHistoryCalculate = new GameHistoryCalculate(fakeGameVendorMember);

        var actual = gameHistoryCalculate
            .Execute(
                operatorId,
                detail);

        Assert.IsNotNull(actual);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.PlayerCode));
        Assert.IsTrue(actual.IsClosed);
        Assert.IsTrue(10m == actual.Bet);
        Assert.IsTrue(10m == actual.ValidBet);
        Assert.IsTrue(20m == actual.Settle);
        Assert.IsTrue(10m == actual.Turnover);
        Assert.IsTrue(10m == actual.PlayerWinLoss);
        Assert.IsTrue(detail.TransactionTime == actual.ExternalCreatedOn);
        Assert.IsTrue(detail.WinlostDatetime == actual.ExternalClosedOn);
    }

    [TestMethod]
    public void Cashout()
    {
        var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

        var operatorId = "tg";
        var detail = new BetDetail(
            TransId: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VendorMemberId: "tg_4ucmqplxb029_test",
            SportType: 1,
            TicketStatus: "Refund",
            TicketExtraStatus: "cashout",
            Stake: 10,
            WinlostAmount: 0,
            BuybackAmount: 10m,
            TransactionTime: DateTime.UtcNow.AddHours(-1),
            SettlementTime: DateTime.UtcNow,
            WinlostDatetime: DateTime.Today,
            AdditionalData: JsonSerializer.Serialize(new { VendorMemberId = "tg_4ucmqplxb029_test" }));

        _ = fakeGameVendorMember
            .ConvertToPlatformMember(
                Arg.Is(operatorId),
                Arg.Is(detail.VendorMemberId))
            .Returns("4ucmqplxb029");

        var gameHistoryCalculate = new GameHistoryCalculate(fakeGameVendorMember);

        var actual = gameHistoryCalculate
            .Execute(
                operatorId,
                detail);

        Assert.IsNotNull(actual);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.PlayerCode));
        Assert.IsTrue(actual.IsCashout);
        Assert.IsTrue(actual.IsClosed);
        Assert.IsTrue(10m == actual.Bet);
        Assert.IsTrue(0m == actual.ValidBet);
        Assert.IsTrue(10m == actual.Settle);
        Assert.IsTrue(0m == actual.Turnover);
        Assert.IsTrue(0m == actual.PlayerWinLoss);
        Assert.IsTrue(detail.TransactionTime == actual.ExternalCreatedOn);
        Assert.IsTrue(detail.SettlementTime == actual.ExternalClosedOn);
    }

    [TestMethod]
    public void Reject()
    {
        var fakeGameVendorMember = Substitute.For<IGameVendorMember>();

        var operatorId = "tg";
        var detail = new BetDetail(
            TransId: DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            VendorMemberId: "tg_4ucmqplxb029_test",
            SportType: 1,
            TicketStatus: "reject",
            TicketExtraStatus: null,
            Stake: 1,
            WinlostAmount: 0,
            BuybackAmount: null,
            TransactionTime: DateTime.Parse("2022-11-02T21:15:18.44-04:00"),
            SettlementTime: DateTime.UnixEpoch,
            WinlostDatetime: DateTime.Parse("2022-11-02T00:00:00-04:00"),
            AdditionalData: "{\"trans_id\":46405051463237660,\"vendor_member_id\":\"tgidr_4uguvssjj02s_test\",\"operator_id\":\"tgidr\",\"league_id\":4679,\"leaguename\":[{\"lang\":\"en\",\"name\":\"COLOMBIA CUP\"},{\"lang\":\"cs\",\"name\":\"哥伦比亚杯\"}],\"match_id\":59853308,\"home_id\":9062,\"hometeamname\":[{\"lang\":\"en\",\"name\":\"Millonarios\"},{\"lang\":\"cs\",\"name\":\"米伦拿列奥\"}],\"away_id\":32425,\"awayteamname\":[{\"lang\":\"en\",\"name\":\"Junior Barranquilla\"},{\"lang\":\"cs\",\"name\":\"巴兰基利亚青年\"}],\"match_datetime\":\"2022-11-02T21:00:00\",\"sport_type\":1,\"sportname\":[{\"lang\":\"en\",\"name\":\"Soccer\"},{\"lang\":\"cs\",\"name\":\"足球\"}],\"bet_type\":15,\"bettypename\":[{\"lang\":\"en\",\"name\":\"1H 1X2\"},{\"lang\":\"cs\",\"name\":\"上半场.独赢盘\"}],\"parlay_ref_no\":0,\"odds\":1.78,\"stake\":1,\"transaction_time\":\"2022-11-02T21:15:18.44\",\"ticket_status\":\"reject\",\"winlost_amount\":0,\"after_amount\":97,\"currency\":20,\"winlost_datetime\":\"2022-11-02T00:00:00\",\"odds_type\":3,\"bet_team\":\"x\",\"isLucky\":\"False\",\"home_hdp\":1.1,\"away_hdp\":1,\"hdp\":null,\"betfrom\":\"x\",\"islive\":\"1\",\"home_score\":0,\"away_score\":0,\"settlement_time\":null,\"customInfo1\":\"\",\"customInfo2\":\"\",\"customInfo3\":\"\",\"customInfo4\":\"\",\"customInfo5\":\"\",\"ba_status\":\"0\",\"version_key\":670749,\"ParlayData\":null}"
            );

        _ = fakeGameVendorMember
            .ConvertToPlatformMember(
                Arg.Is(operatorId),
                Arg.Is(detail.VendorMemberId))
            .Returns("4ucmqplxb029");

        var gameHistoryCalculate = new GameHistoryCalculate(fakeGameVendorMember);

        var actual = gameHistoryCalculate
            .Execute(
                operatorId,
                detail);

        Assert.IsNotNull(actual);
        Assert.IsTrue("4ucmqplxb029".Equals(actual.PlayerCode));
        Assert.IsFalse(actual.IsCashout);
        Assert.IsTrue(actual.IsClosed);
        Assert.IsTrue(1m == actual.Bet);
        Assert.IsTrue(0m == actual.ValidBet);
        Assert.IsTrue(0m == actual.Settle);
        Assert.IsTrue(0m == actual.Turnover);
        Assert.IsTrue(0m == actual.PlayerWinLoss);
        Assert.IsTrue(detail.TransactionTime == actual.ExternalCreatedOn);
        Assert.IsTrue(detail.WinlostDatetime == actual.ExternalClosedOn);
    }
}
