using Tgs.GameInterface.SabaService.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public class StatementReprortGetService(IStatementReportApiClient statementReportApiClient) : IGetService<StatementReportQuery, StatementReportResult>
{
	public async ValueTask<StatementReportResult> GetAsync(StatementReportQuery query, CancellationToken cancellationToken = default)
	{
		var token = await statementReportApiClient.GetReportTokenAsync(query.ApiUrl, new StatementTokenRequest()
		{
			Username = query.Username,
			Password = query.Password,
		}, cancellationToken).ConfigureAwait(false);

		var url = $"{query.ApiUrl.AbsoluteUri}BetList/Refno?access_token={token.AccessToken}&refid={query.TransactionId}";

		if (query.BaseCurrency is not null)
		{
			url += $"&bc={query.BaseCurrency}";
		}

		// TO DO: For RNG Keno, Lottery, Table Game, LG Virtual Sports: Please provide username.
		if (query.PlayerUsername is not null)
		{
			url += $"&urs={query.PlayerUsername}";
		}

		if (query.Language is not null)
		{
			url += $"&lang={query.Language}";
		}

		return new StatementReportResult() { Url = url };
	}
}
