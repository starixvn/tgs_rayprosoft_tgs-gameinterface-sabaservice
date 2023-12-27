using System.Text.Json;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class GameHistoryUnsettledByTimeframeQueryServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();
        var fakeGameHistoryCalculate = Substitute.For<IGameHistoryCalculate>();

        var query = new GameHistoryUnsettledByTimeframeQuery(
            ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
            OperatorId: "tg",
            VendorId: "um5vspnzkx",
            StartTime: DateTime.Parse("2022-10-27T15:50:00+08:00"),
            EndTime: DateTime.Parse("2022-10-27T15:55:00+08:00"));

        var result = new BetDetail[]
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
            };
        _ = fakeGameApiClient
            .GetBetDetailByTimeframeAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                timeType: Arg.Is(GameTimeType.Bet),
                startTime: Arg.Is(query.StartTime),
                endTime: Arg.Is(query.EndTime))
            .Returns(result.ToAsyncEnumerable());

        var sut = new GameHistoryUnsettledByTimeframeQueryService(
            fakeGameApiClient,
            fakeGameHistoryCalculate);

        var actual = await sut
            .QueryAsync(query)
            .ToArrayAsync()
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameHistoryDetail>();

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Any());
    }
}
