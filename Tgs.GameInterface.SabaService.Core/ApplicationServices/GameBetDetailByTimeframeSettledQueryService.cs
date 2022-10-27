using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameBetDetailByTimeframeSettledQueryService : IQueryService<GameBetDetailByTimeframeSettledQuery, GameBetDetail>
{
    private readonly IGameApiClient _client;
    private readonly IVendorMember _vendorMember;

    public GameBetDetailByTimeframeSettledQueryService(
        IGameApiClient client,
        IVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async IAsyncEnumerable<GameBetDetail> QueryAsync(
        GameBetDetailByTimeframeSettledQuery query,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var betDetails = await _client
            .GetBetDetailByTimeframeAsync(
                apiUrl: query.ApiUrl,
                vendorId: query.VendorId,
                timeType: GameTimeType.Settle,
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
            var externalClosedOn = betDetail.SportType < 100 ? betDetail.SettlementTime!.Value : betDetail.WinlostDatetime!.Value;

            yield return new GameBetDetail(
                PlayerCode: playerCode,
                ExternalRoundId: betDetail.TransId.ToString(),
                IsClosed: true,
                Status: betDetail.TicketStatus,
                ExternalCreatedOn: betDetail.TransactionTime,
                ExternalClosedOn: externalClosedOn,
                Bet: betDetail.Stake,
                ValidBet: betDetail.Stake,
                PlayerWinLoss: betDetail.WinlostAmount,
                Turnover: betDetail.Stake,
                Settle: betDetail.BuybackAmount ?? betDetail.WinlostAmount + betDetail.Stake,
                AdditionalData: betDetail.AdditionalData);
        }
    }
}
