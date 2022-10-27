using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameBetDetailByTimeframeUnsettledQueryService : IQueryService<GameBetDetailByTimeframeSettledQuery, GameBetDetail>
{
    private readonly IGameApiClient _client;
    private readonly IVendorMember _vendorMember;

    public GameBetDetailByTimeframeUnsettledQueryService(
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

            yield return new GameBetDetail(
                PlayerCode: playerCode,
                ExternalRoundId: betDetail.TransId.ToString(),
                IsClosed: false,
                Status: GameTicketStatus.Running,
                ExternalCreatedOn: betDetail.TransactionTime,
                ExternalClosedOn: DateTime.UnixEpoch,
                Bet: betDetail.Stake,
                ValidBet: betDetail.Stake,
                PlayerWinLoss: 0,
                Turnover: 0,
                Settle: 0,
                AdditionalData: betDetail.AdditionalData);
        }
    }
}
