using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using Tgs.GameCommand.Client;
using Tgs.GameInterface.SabaService.Core.Models;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public class StatementReprortGetService(IOptions<AppSettings> appSettings, IGameCommandClient gameCommandClient, IStatementReportApiClient statementReportApiClient) : IGetService<StatementReportQuery, StatementReportResult>
{
	private AppSettings AppSettings = appSettings.Value;

	public async ValueTask<StatementReportResult> GetAsync(StatementReportQuery query, CancellationToken cancellationToken = default)
	{
		var configuration = await gameCommandClient.SettingGetConfigurationAsync<SabaSWConfiguration>(key: AppSettings.ProviderConfiguration, cancellationToken: cancellationToken).ConfigureAwait(false);

		var content = JsonContent.Create(new StatementTokenRequest(
			Username: configuration!.Username,
			Password: configuration.Password
		));

		var token = await statementReportApiClient.PostAsync<StatementTokenResponse>(new StatementReportRequest(
			ApiUrl: configuration!.ReportUrl,
			Path: "api/auth",
			Content: content,
			cancellationToken)).ConfigureAwait(false);

		var url = $"{configuration.ReportUrl}BetList/Refno?access_token={token.AccessToken}&refid={query.TransactionId}";

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
