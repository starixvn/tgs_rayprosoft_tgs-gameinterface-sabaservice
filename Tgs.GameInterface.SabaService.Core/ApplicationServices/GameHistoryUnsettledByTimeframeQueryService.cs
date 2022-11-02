using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameHistoryUnsettledByTimeframeQueryService : IQueryService<GameHistoryUnsettledByTimeframeQuery, GameHistoryDetail>
{
    private readonly IGameApiClient _client;
    private readonly IVendorMember _vendorMember;

    public GameHistoryUnsettledByTimeframeQueryService(
        IGameApiClient client,
        IVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async IAsyncEnumerable<GameHistoryDetail> QueryAsync(
        GameHistoryUnsettledByTimeframeQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var betDetails = await _client
            .GetBetDetailByTimeframeAsync(
                apiUrl: query.ApiUrl,
                vendorId: query.VendorId,
                timeType: GameTimeType.Bet,
                startTime: query.StartTime,
                endTime: query.EndTime,
                cancellationToken: cancellationToken)
            .ToArrayAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!betDetails.Any())
        {
            yield break;
        }

        foreach (var betDetail in betDetails)
        {
            var playerCode = _vendorMember.ConvertToPlatformMember(query.OperatorId, betDetail.VendorMemberId);
            var isClosed = !betDetail.TicketStatus.Equals("running", StringComparison.OrdinalIgnoreCase);
            var externalClosedOn = DateTime.UnixEpoch;
            var validBet = betDetail.Stake;
            var playerWinLoss = 0m;
            var turnover = 0m;
            var settle = 0m;
            var isCashout = betDetail.BuybackAmount.HasValue;
            if (isClosed)
            {
                validBet = isCashout ? 0 : betDetail.Stake;
                playerWinLoss = betDetail.WinlostAmount;
                turnover = isCashout ? 0 : betDetail.Stake;
                settle = betDetail.BuybackAmount ?? betDetail.WinlostAmount + betDetail.Stake;
                externalClosedOn = betDetail.SportType > 99
                    ? betDetail.WinlostDatetime
                    : betDetail.SettlementTime.ToUniversalTime() != DateTime.UnixEpoch
                    ? betDetail.SettlementTime
                    : betDetail.WinlostDatetime;
            }

            yield return new GameHistoryDetail(
                    PlayerCode: playerCode,
                    ExternalRoundId: betDetail.TransId.ToString(),
                    IsCashout: isCashout,
                    IsClosed: isClosed,
                    Status: betDetail.TicketStatus,
                    ExternalCreatedOn: betDetail.TransactionTime,
                    ExternalClosedOn: externalClosedOn,
                    Bet: betDetail.Stake,
                    ValidBet: validBet,
                    PlayerWinLoss: playerWinLoss,
                    Turnover: turnover,
                    Settle: settle,
                    AdditionalData: betDetail.AdditionalData);
        }
    }
}
