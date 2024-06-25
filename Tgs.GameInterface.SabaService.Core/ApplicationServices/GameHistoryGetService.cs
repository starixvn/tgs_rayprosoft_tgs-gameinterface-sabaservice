using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameHistoryGetService(
	IGameApiClient client,
	IGameHistoryCalculate calculate) : IGetService<GameHistoryQuery, GameHistoryResult>
{
	public async ValueTask<GameHistoryResult> GetAsync(
		GameHistoryQuery query,
		CancellationToken cancellationToken = default)
	{
		var result = await client
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
				return calculate.Execute(query.OperatorId, betDetail);
			});

		return new GameHistoryResult(
			LastVersionKey: result.LastVersionKey,
			Details: betDetails);
	}
}
