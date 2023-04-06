using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameHistoryUnsettledByTimeframeQueryService : IQueryService<GameHistoryUnsettledByTimeframeQuery, GameHistoryDetail>
{
    private readonly IGameApiClient _client;
    private readonly IGameHistoryCalculate _calculate;

    public GameHistoryUnsettledByTimeframeQueryService(
        IGameApiClient client,
        IGameHistoryCalculate calculate)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _calculate = calculate ?? throw new ArgumentNullException(nameof(calculate));
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
            yield return _calculate.Execute(query.OperatorId, betDetail);
        }
    }
}
