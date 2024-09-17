using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

internal class StatementReportApiClient(ISentryClient sentryClient, HttpClient client) : IStatementReportApiClient
{
	public async Task<StatementTokenResponse> GetReportTokenAsync(Uri apiUrl, StatementTokenRequest request, CancellationToken cancellationToken = default)
	{
		var sw = Stopwatch.StartNew();

		client.BaseAddress = apiUrl;
		var response = await client.PostAsync("api/auth", JsonContent.Create(request), cancellationToken).ConfigureAwait(false);

		sw.Stop();

		_ = response.EnsureSuccessStatusCode();

		var data = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		WriteLog(JsonSerializer.Serialize(request), JsonSerializer.Serialize(response));

		return JsonSerializer.Deserialize<StatementTokenResponse>(data)!;
	}

	private void WriteLog(string request, string response)
	{
		sentryClient.CaptureMessage(string.Join("/r/n", "GetToken Details:", request, response));
	}
}
