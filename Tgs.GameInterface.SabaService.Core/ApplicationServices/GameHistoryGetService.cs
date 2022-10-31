using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameHistoryGetService : IGetService<GameHistoryQuery, GameHistoryResult>
{
    private readonly IGameApiClient _client;
    private readonly IVendorMember _vendorMember;

    public GameHistoryGetService(
        IGameApiClient client,
        IVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async ValueTask<GameHistoryResult> GetAsync(
        GameHistoryQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _client
            .GetBetDetailAsync(
                apiUrl: query.ApiUrl,
                vendorId: query.VendorId,
                versionKey: query.VersionKey,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!result.Details.Any())
        {
            return new GameHistoryResult(
                LastVersionKey: result.LastVersionKey,
                Details: Enumerable.Empty<GameHistoryDetail>());
        }

        var betDetails = result.Details.Where(x => !x.TicketStatus.Equals("waiting", StringComparison.OrdinalIgnoreCase))
            .Select(betDetail =>
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
                    externalClosedOn = betDetail.SportType < 100 ? betDetail.SettlementTime : betDetail.WinlostDatetime;
                }

                return new GameHistoryDetail(
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
            });

        return new GameHistoryResult(
            LastVersionKey: result.LastVersionKey,
            Details: betDetails);
    }
}
