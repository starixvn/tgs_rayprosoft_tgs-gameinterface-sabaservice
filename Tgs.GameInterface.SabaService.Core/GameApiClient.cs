using System.Diagnostics;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Exceptions;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

internal class GameApiClient(
	HttpClient client,
	ILogger<GameApiClient> logger) : IGameApiClient
{
	public async ValueTask CreateMemberAsync(Uri apiUrl, string operatorId, string vendorId, string vendorMemberId, int oddsType, int currency, decimal maxTransfer = 9999999999, decimal minTransfer = 0, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["vendor_member_id"] = vendorMemberId,
			["operatorId"] = operatorId,
			["userName"] = vendorMemberId,
			["oddsType"] = oddsType.ToString(),
			["currency"] = currency.ToString(),
			["maxTransfer"] = maxTransfer.ToString(),
			["minTransfer"] = ((int)minTransfer).ToString()
		};

		var url = new Uri(apiUrl, "/api/CreateMember/");

		var jsonNode = await PostAsync(
			apiUrl: url,
			parameters: parameters,
			logPrefix: nameof(CreateMemberAsync),
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
		{
			if (jsonNode["error_code"]?.GetValue<int>() is null or not 2 and not 6)
			{
				throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
			}
		}
	}

	public async ValueTask<GetGameUrlResult> GetSabaUrlAsync(Uri apiUrl, string vendorId, string vendorMemberId, GamePlatformType platformType, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["platform"] = ((int)platformType).ToString(),
			["vendor_member_id"] = vendorMemberId
		};

		var url = new Uri(apiUrl, "/api/GetSabaUrl/");

		var jsonNode = await PostAsync(
		   apiUrl: url,
		   parameters: parameters,
		   logPrefix: nameof(GetSabaUrlAsync),
		   cancellationToken: cancellationToken).ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
		{
			return (jsonNode["error_code"]?.GetValue<int>() is null or not 2)
				? throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString())
				: new GetGameUrlResult(false, string.Empty);
		}

		var data = jsonNode["Data"]?.ToString()
			?? throw new Exception("取得網址失敗");

		return new GetGameUrlResult(true, data);
	}

	public async ValueTask<StatementTokenResult> GetStatementReportToken(Uri apiUrl, string username, string password, CancellationToken cancellationToken = default)
	{
		var url = new Uri(apiUrl, "/api/auth/");

		var content = JsonContent.Create(new
		{
			username,
			password,
		});

		var token = await PostAsync<StatementTokenResult>(
		   apiUrl: url,
		   content: content,
		   logPrefix: nameof(GetStatementReportToken),
		   cancellationToken: cancellationToken).ConfigureAwait(false);

		return token;
	}

	private async Task<TResult> PostAsync<TResult>(Uri apiUrl, JsonContent content, string logPrefix, CancellationToken cancellationToken = default)
	{
		var sw = Stopwatch.StartNew();

		var response = await client.PostAsync(apiUrl, content, cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		sw.Stop();

		var data = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

		var apiLogEvent = new
		{
			Path = response.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = response.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = response.RequestMessage?.Headers + response.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = (await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
			ResponseHeaders = response.Headers.ToString(),
			ResponseContent = data.NoLongerThan(4096),
			ResponseCode = (short)response.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};

		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		return JsonSerializer.Deserialize<TResult>(data)!;
	}

	private async ValueTask<JsonNode> PostAsync(Uri apiUrl, IEnumerable<KeyValuePair<string, string>> parameters, string logPrefix, CancellationToken cancellationToken = default)
	{
		var sw = Stopwatch.StartNew();

		HttpContent content = new FormUrlEncodedContent(parameters);

		var httpResponseMessage = await client.PostAsync(apiUrl, content, cancellationToken).ConfigureAwait(false);

		sw.Stop();

		var responseMessage = default(string);

		using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
		{
			responseMessage = await sr
				.ReadToEndAsync(
					cancellationToken: cancellationToken)
				.ConfigureAwait(false) ?? string.Empty;
		}

		var apiLogEvent = new
		{
			Path = httpResponseMessage.RequestMessage?.RequestUri?.AbsoluteUri.NoLongerThan(512) ?? string.Empty,
			Method = httpResponseMessage.RequestMessage?.Method.ToString().NoLongerThan(8) ?? string.Empty,
			Prefix = logPrefix,
			ServerName = Environment.MachineName.NoLongerThan(128),
			RequestHeaders = httpResponseMessage.RequestMessage?.Headers + httpResponseMessage.RequestMessage?.Content?.Headers?.ToString(),
			RequestContent = (await content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false)).NoLongerThan(4096),
			ResponseHeaders = httpResponseMessage.Headers.ToString(),
			ResponseContent = responseMessage.NoLongerThan(4096),
			ResponseCode = (short)httpResponseMessage.StatusCode,
			ExecutionTimeMs = (int)sw.ElapsedMilliseconds,
			RecordedOn = DateTimeOffset.UtcNow
		};

		logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

		_ = httpResponseMessage.EnsureSuccessStatusCode();

		return JsonNode.Parse(responseMessage)!;
	}
}
