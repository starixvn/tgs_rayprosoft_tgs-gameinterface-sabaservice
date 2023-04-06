using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameHistoryGetService : IGetService<GameHistoryQuery, GameHistoryResult>
{
    private readonly IGameApiClient _client;
    private readonly IGameHistoryCalculate _calculate;

    public GameHistoryGetService(
        IGameApiClient client,
        IGameHistoryCalculate calculate)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _calculate = calculate ?? throw new ArgumentNullException(nameof(calculate));
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
                return _calculate.Execute(query.OperatorId, betDetail);
            });

        return new GameHistoryResult(
            LastVersionKey: result.LastVersionKey,
            Details: betDetails);
    }
}
