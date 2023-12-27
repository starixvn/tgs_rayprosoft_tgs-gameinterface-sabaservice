using System.Text.Json;
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
        var fakeGameHistoryCalculate = Substitute.For<IGameHistoryCalculate>();

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new(
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
                versionKey: Arg.Is(query.VersionKey),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var service = new GameHistoryGetService(
            fakeGameApiClient,
            fakeGameHistoryCalculate);

        var actual = await service
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
        var fakeGameHistoryCalculate = Substitute.For<IGameHistoryCalculate>();

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new(
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
                versionKey: Arg.Is(query.VersionKey),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var service = new GameHistoryGetService(
            fakeGameApiClient,
            fakeGameHistoryCalculate);

        var actual = await service
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
    }

    [TestMethod]
    public async Task Unsettled()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();
        var fakeGameHistoryCalculate = Substitute.For<IGameHistoryCalculate>();

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new(
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
                versionKey: Arg.Is(query.VersionKey),
                cancellationToken: Arg.Any<CancellationToken>())
            .Returns(result);

        var service = new GameHistoryGetService(
            fakeGameApiClient,
            fakeGameHistoryCalculate);

        var actual = await service
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
    }

    [TestMethod]
    public async Task 取得資料同時有Waiting_Unsettled_Settled()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();
        var fakeGameHistoryCalculate = Substitute.For<IGameHistoryCalculate>();

        var query = new GameHistoryQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            VersionKey: 1);

        var result = new GetBetDetailResult(
            LastVersionKey: 2000,
            Details: new BetDetail[]
            {
                new(
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
                new(
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
                new(
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
                    versionKey: Arg.Is(query.VersionKey),
                    cancellationToken: Arg.Any<CancellationToken>())
                .Returns(result);

        var service = new GameHistoryGetService(
            fakeGameApiClient,
            fakeGameHistoryCalculate);

        var actual = await service
            .GetAsync(query)
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.LastVersionKey == 2000);
        Assert.IsTrue(actual.Details.Any());
        Assert.IsTrue(actual.Details.Count() == 2);
    }
}
