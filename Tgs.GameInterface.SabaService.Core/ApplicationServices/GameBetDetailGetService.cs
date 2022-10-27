using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameBetDetailGetService : IGetService<GameBetDetailQuery, GameBetDetailResult>
{
    private readonly IGameApiClient _client;
    private readonly IVendorMember _vendorMember;

    public GameBetDetailGetService(
        IGameApiClient client,
        IVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async ValueTask<GameBetDetailResult> GetAsync(
        GameBetDetailQuery query,
        CancellationToken cancellationToken = default)
    {
        var result = await _client
            .GetBetDetailAsync(
                apiUrl: query.ApiUrl,
                vendorId: query.VendorId,
                versionKey: query.VersionKey,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        if (!result.BetDetails.Any())
        {
            return new GameBetDetailResult(
                LastVersionKey: result.LastVersionKey,
                BetDetails: Enumerable.Empty<GameBetDetail>());
        }

        var betDetails = result.BetDetails.Where(x => x.TicketStatus != GameTicketStatus.Waiting)
            .Select(betDetail =>
            {
                var playerCode = _vendorMember.ConvertToPlatformMember(query.OperatorId, betDetail.VendorMemberId);
                var isClosed = betDetail.TicketStatus != GameTicketStatus.Running;
                var externalClosedOn = DateTime.UnixEpoch;
                var playerWinLoss = 0m;
                var turnover = 0m;
                var settle = 0m;

                if (isClosed)
                {
                    playerWinLoss = betDetail.WinlostAmount;
                    turnover = betDetail.Stake;
                    settle = betDetail.BuybackAmount ?? betDetail.WinlostAmount + betDetail.Stake;
                    externalClosedOn = betDetail.SportType < 100 ? betDetail.SettlementTime!.Value : betDetail.WinlostDatetime!.Value;
                }

                return new GameBetDetail(
                    PlayerCode: playerCode,
                    ExternalRoundId: betDetail.TransId.ToString(),
                    IsClosed: isClosed,
                    Status: betDetail.TicketStatus,
                    ExternalCreatedOn: betDetail.TransactionTime,
                    ExternalClosedOn: externalClosedOn,
                    Bet: betDetail.Stake,
                    ValidBet: betDetail.Stake,
                    PlayerWinLoss: playerWinLoss,
                    Turnover: betDetail.Stake,
                    Settle: settle,
                    AdditionalData: betDetail.AdditionalData);
            });

        return new GameBetDetailResult(
            LastVersionKey: result.LastVersionKey,
            BetDetails: betDetails);
    }
}
