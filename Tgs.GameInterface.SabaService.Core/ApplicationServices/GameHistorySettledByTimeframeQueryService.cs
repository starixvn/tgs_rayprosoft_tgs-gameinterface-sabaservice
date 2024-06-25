using System.Runtime.CompilerServices;
using Tgs.GameInterface.SabaService.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameHistorySettledByTimeframeQueryService(
	IGameApiClient client,
	IGameHistoryCalculate calculate) : IQueryService<GameHistorySettledByTimeframeQuery, GameHistoryDetail>
{
	public async IAsyncEnumerable<GameHistoryDetail> QueryAsync(
		GameHistorySettledByTimeframeQuery query,
		[EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var betDetails = await client
			.GetBetDetailByTimeframeAsync(
				apiUrl: query.ApiUrl,
				vendorId: query.VendorId,
				timeType: GameTimeType.Settle,
				startTime: query.StartTime,
				endTime: query.EndTime,
				cancellationToken: cancellationToken)
			.ToArrayAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<BetDetail>();

		if (!betDetails.Any())
		{
			yield break;
		}

		foreach (var betDetail in betDetails)
		{
			yield return calculate.Execute(query.OperatorId, betDetail);
		}
	}
}
