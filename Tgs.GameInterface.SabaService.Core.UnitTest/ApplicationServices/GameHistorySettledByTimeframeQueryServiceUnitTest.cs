using System.Text.Json;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core.UnitTest.ApplicationServices;

[TestClass]
public class GameHistorySettledByTimeframeQueryServiceUnitTest
{
    [TestMethod]
    public async Task Normal_Process()
    {
        var fakeGameApiClient = Substitute.For<IGameApiClient>();

        var vendorMember = new VendorMember(
            Options.Create(new GameInterfaceSettingsOptions()
            {
                ReleaseNamespace = "dev"
            }));

        var gameHistoryCalculate = new GameHistoryCalculate(vendorMember);

        var query = new GameHistorySettledByTimeframeQuery(
                ApiUrl: new Uri("http://h0i2tsa.bw6688.com/"),
                OperatorId: "tg",
                VendorId: "um5vspnzkx",
                StartTime: DateTime.Parse("2022-10-27T15:50:00+08:00"),
                EndTime: DateTime.Parse("2022-10-27T15:55:00+08:00"));

        var result = new BetDetail[]
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
            };
        _ = fakeGameApiClient
            .GetBetDetailByTimeframeAsync(
                apiUrl: Arg.Is(query.ApiUrl),
                vendorId: Arg.Is(query.VendorId),
                timeType: Arg.Is(GameTimeType.Settle),
                startTime: Arg.Is(query.StartTime),
                endTime: Arg.Is(query.EndTime))
            .Returns(result.ToAsyncEnumerable());

        var sut = new GameHistorySettledByTimeframeQueryService(
            fakeGameApiClient,
            gameHistoryCalculate);

        var actual = await sut
            .QueryAsync(query)
            .ToArrayAsync()
            .ConfigureAwait(false);

        Assert.IsNotNull(actual);
        Assert.IsTrue(actual.Any());
        Assert.IsTrue("4ucmqplxb029".Equals(actual.First().PlayerCode));
        Assert.IsTrue(10m == actual.First().ValidBet);
        Assert.IsTrue(10m == actual.First().Turnover);
        Assert.IsTrue(20m == actual.First().Settle);
        Assert.IsTrue(result.First().TransactionTime == actual.First().ExternalCreatedOn);
        Assert.IsTrue(result.First().SettlementTime == actual.First().ExternalClosedOn);
    }
}
