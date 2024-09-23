using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public class StatementReportTokenGetService(IGameApiClient gameApiClient) : IGetService<StatementReportTokenQuery, StatementTokenResult>
{
	public async ValueTask<StatementTokenResult> GetAsync(StatementReportTokenQuery query, CancellationToken cancellationToken = default)
	{
		var token = await gameApiClient.GetStatementReportToken(query.ApiUrl, query.Username, query.Password, cancellationToken).ConfigureAwait(false);

		return token;
	}
}
