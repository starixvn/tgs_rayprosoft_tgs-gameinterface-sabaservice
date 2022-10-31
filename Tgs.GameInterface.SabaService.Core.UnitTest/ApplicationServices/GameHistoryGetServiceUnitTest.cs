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
        var fakeVendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

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
            fakeVendorMember);

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
        var fakeVendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

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
            fakeVendorMember);

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
        var fakeVendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

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
            fakeVendorMember);

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
        var fakeVendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

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
            fakeVendorMember);

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
        var fakeVendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

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
            fakeVendorMember);

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
        var fakeVendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

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
            fakeVendorMember);

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
}
