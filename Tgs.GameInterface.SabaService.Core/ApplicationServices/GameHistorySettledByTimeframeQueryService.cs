using System.Runtime.CompilerServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameHistorySettledByTimeframeQueryService : IQueryService<GameHistorySettledByTimeframeQuery, GameHistoryDetail>
{
    private readonly IGameApiClient _client;
    private readonly GameHistoryCalculate _calculate;

    public GameHistorySettledByTimeframeQueryService(
        IGameApiClient client,
        GameHistoryCalculate calculate)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _calculate = calculate ?? throw new ArgumentNullException(nameof(calculate));
    }

    public async IAsyncEnumerable<GameHistoryDetail> QueryAsync(
        GameHistorySettledByTimeframeQuery query,
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
            yield return _calculate.Process(query.OperatorId, betDetail);
        }
    }
}
