using System.Text.Json;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

internal class StatementReportApiClient(ISentryClient sentryClient, HttpClient client) : IStatementReportApiClient
{
	public async Task<TResult> PostAsync<TResult>(StatementReportRequest request)
	{
		client.BaseAddress = request.ApiUrl;
		var response = await client.PostAsync(request.Path, request.Content, request.CancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		var data = await response.Content.ReadAsStringAsync(request.CancellationToken).ConfigureAwait(false);

		var requestContent = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
		WriteLog(requestContent, data);

		return JsonSerializer.Deserialize<TResult>(data)!;
	}

	private void WriteLog(string request, string response)
	{
		sentryClient.CaptureMessage(string.Join("/r/n", "GetToken Details:", request, response));
	}
}
