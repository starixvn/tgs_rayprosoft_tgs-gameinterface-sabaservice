using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
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
	public async ValueTask<CheckFundTransferResult> CheckFundTransferAsync(Uri apiUrl, string vendorId, string vendorTransId, GameWalletType walletType, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["vendor_trans_id"] = vendorTransId,
			["wallet_id"] = ((int)walletType).ToString()
		};

		var url = new Uri(apiUrl, "/api/CheckFundTransfer/");

		var jsonNode = await PostAsync(
				apiUrl: url,
				parameters: parameters,
				logPrefix: nameof(CheckFundTransferAsync),
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0 and not 3)
		{
			throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
		}

		if (jsonNode["error_code"]?.GetValue<int>() is 3)
		{
			return new CheckFundTransferResult(
				Retry: true,
				IsOrderExist: false);
		}

		var data = jsonNode["Data"]
			?? throw new Exception("取得資料失敗");

		return data["status"]?.GetValue<int>() is 2
			? new CheckFundTransferResult(
				Retry: true,
				IsOrderExist: false)
			: new CheckFundTransferResult(
				Retry: false,
				IsOrderExist: data["status"]?.GetValue<int>() is 0);
	}

	public async ValueTask<GetBalanceResult> CheckUserBalanceAsync(Uri apiUrl, string vendorId, string vendorMemberId, GameWalletType walletType, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["vendor_member_ids"] = vendorMemberId,
			["wallet_id"] = ((int)walletType).ToString()
		};

		var url = new Uri(apiUrl, "/api/CheckUserBalance/");

		var jsonNode = await PostAsync(
			apiUrl: url,
			parameters: parameters,
			logPrefix: nameof(CheckUserBalanceAsync),
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
		{
			throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
		}

		var data = jsonNode["Data"]?.AsArray().FirstOrDefault()
			?? throw new Exception("取得點數失敗");

		var error_code = data["error_code"]?.GetValue<int>()
			?? throw new Exception("取得錯誤代碼為空值，造成取得點數失敗");

		if (error_code is not 0 and not 6)
		{
			return (error_code is not 2)
				? throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString())
				: new GetBalanceResult(
					IsPlayerExist: false,
					Balance: 0,
					Outstanding: 0);
		}

		var balance = error_code is 6
			? 0
			: data["balance"]?.GetValue<decimal>()
			 ?? throw new Exception("取得點數失敗");

		var outstanding = error_code is 6
			? 0
			: data["outstanding"]?.GetValue<decimal>()
			 ?? throw new Exception("取得點數失敗");

		return new GetBalanceResult(true, balance, outstanding);
	}

	public async ValueTask CreateMemberAsync(Uri apiUrl, string operatorId, string vendorId, string vendorMemberId, string firstName, string lastName, int oddsType, int currency, decimal maxTransfer = 9999999999, decimal minTransfer = 0, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["vendor_member_id"] = vendorMemberId,
			["operatorId"] = operatorId,
			["firstName"] = firstName,
			["lastName"] = lastName,
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

	public async ValueTask<FundTransferResult> FundTransferAsync(Uri apiUrl, string vendorId, string vendorMemberId, string vendorTransId, GameDirectionType directionType, GameWalletType walletType, int currency, decimal amount, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["vendor_member_id"] = vendorMemberId,
			["vendor_trans_id"] = vendorTransId,
			["amount"] = amount.ToString(),
			["currency"] = currency.ToString(),
			["direction"] = ((int)directionType).ToString(),
			["wallet_id"] = ((int)walletType).ToString()
		};

		var url = new Uri(apiUrl, "/api/FundTransfer/");

		var jsonNode = await PostAsync(
			apiUrl: url,
			parameters: parameters,
			logPrefix: nameof(FundTransferAsync),
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0 and not 1)
		{
			throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
		}

		if (jsonNode["error_code"]?.GetValue<int>() is 1)
		{
			return new FundTransferResult(
				 Check: true,
				 IsOrderExist: false,
				 TransId: 0,
				 Resp: jsonNode.ToJsonString());
		}

		var data = jsonNode["Data"]
			?? throw new Exception("取得資料失敗");

		return new FundTransferResult(
			Check: false,
			IsOrderExist: data["status"]!.GetValue<int>() == 0,
			TransId: data["trans_id"]!.GetValue<long>(),
			Resp: data.ToJsonString());
	}

	public async ValueTask<GetBetDetailResult> GetBetDetailAsync(Uri apiUrl, string vendorId, long versionKey, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["version_key"] = versionKey.ToString(),
		};

		var url = new Uri(apiUrl, "/api/GetBetDetail/");

		var jsonNode = await PostAsync(
			apiUrl: url,
			parameters: parameters,
			logPrefix: nameof(GetBetDetailAsync),
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
		{
			throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
		}

		var data = jsonNode["Data"]
			?? throw new Exception("取得Data為null");

		var lastVersionKey = data["last_version_key"]?.GetValue<long>()
			?? throw new Exception("取得last_version_key為空值");

		IEnumerable<BetDetail> betDetails = [];

		var array = data["BetDetails"]?.AsArray();

		if (array is not null && array.Count != 0)
		{
			betDetails = betDetails.Concat(array.Select(item => new BetDetail(
				TransId: item!["trans_id"]!.GetValue<long>(),
				VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
				SportType: item!["sport_type"]?.GetValue<int>() ?? 0,
				TicketStatus: item!["ticket_status"]!.GetValue<string>(),
				TicketExtraStatus: item["ticket_extra_status"]?.ToString(),
				Stake: item!["stake"]!.GetValue<decimal>(),
				WinlostAmount: item!["winlost_amount"]!.GetValue<decimal>(),
				BuybackAmount: item!["buyback_amount"]?.GetValue<decimal>(),
				TransactionTime: (item!["transaction_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				SettlementTime: (item!["settlement_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				WinlostDatetime: (item!["winlost_datetime"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				AdditionalData: item.ToJsonString())));
		}

		array = data["BetNumberDetails"]?.AsArray();

		if (array is not null && array.Count != 0)
		{
			betDetails = betDetails.Concat(array.Select(item => new BetDetail(
				TransId: item!["trans_id"]!.GetValue<long>(),
				VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
				 SportType: item!["sport_type"]?.GetValue<int>() ?? 0,
				TicketStatus: item!["ticket_status"]!.GetValue<string>(),
				TicketExtraStatus: item["ticket_extra_status"]?.ToString(),
				Stake: item!["stake"]!.GetValue<decimal>(),
				WinlostAmount: item!["winlost_amount"]!.GetValue<decimal>(),
				BuybackAmount: item!["buyback_amount"]?.GetValue<decimal>(),
				TransactionTime: (item!["transaction_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				SettlementTime: (item!["settlement_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				WinlostDatetime: (item!["winlost_datetime"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				AdditionalData: item.ToJsonString())));
		}

		array = data["BetVirtualSportDetails"]?.AsArray();

		if (array is not null && array.Count != 0)
		{
			betDetails = betDetails.Concat(array.Select(item => new BetDetail(
				TransId: item!["trans_id"]!.GetValue<long>(),
				VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
				SportType: item!["sport_type"]?.GetValue<int>() ?? 0,
				TicketStatus: item!["ticket_status"]!.GetValue<string>(),
				TicketExtraStatus: item["ticket_extra_status"]?.ToString(),
				Stake: item!["stake"]!.GetValue<decimal>(),
				WinlostAmount: item!["winlost_amount"]!.GetValue<decimal>(),
				BuybackAmount: item!["buyback_amount"]?.GetValue<decimal>(),
				TransactionTime: (item!["transaction_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				SettlementTime: (item!["settlement_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				WinlostDatetime: (item!["winlost_datetime"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				AdditionalData: item.ToJsonString())));
		}

		return new GetBetDetailResult(
			LastVersionKey: lastVersionKey,
			Details: betDetails);
	}

	public async IAsyncEnumerable<BetDetail> GetBetDetailByTimeframeAsync(Uri apiUrl, string vendorId, GameTimeType timeType, DateTime startTime, DateTime endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["start_date"] = startTime.DatetimeConvertToString(),
			["end_date"] = endTime.DatetimeConvertToString(),
			["time_type"] = ((int)timeType).ToString()
		};

		var url = new Uri(apiUrl, "/api/GetBetDetailByTimeframe/");

		var jsonNode = await PostAsync(
			apiUrl: url,
			parameters: parameters,
			logPrefix: nameof(GetBetDetailByTimeframeAsync),
			cancellationToken: cancellationToken).ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
		{
			throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
		}

		var data = jsonNode["Data"];

		if (data is null)
		{
			yield break;
		}

		IEnumerable<BetDetail> betDetails = [];

		var array = data["BetDetails"]?.AsArray();
		if (array is not null && array.Count != 0)
		{
			betDetails = betDetails.Concat(array.Select(item => new BetDetail(
				TransId: item!["trans_id"]!.GetValue<long>(),
				VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
				SportType: item!["sport_type"]?.GetValue<int>() ?? 0,
				TicketStatus: item!["ticket_status"]!.GetValue<string>(),
				TicketExtraStatus: item["ticket_extra_status"]?.ToString(),
				Stake: item!["stake"]!.GetValue<decimal>(),
				WinlostAmount: item!["winlost_amount"]!.GetValue<decimal>(),
				BuybackAmount: item!["buyback_amount"]?.GetValue<decimal>(),
				TransactionTime: (item!["transaction_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				SettlementTime: (item!["settlement_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				WinlostDatetime: (item!["winlost_datetime"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				AdditionalData: item.ToJsonString())));
		}

		array = data["BetNumberDetails"]?.AsArray();
		if (array is not null && array.Count != 0)
		{
			betDetails = betDetails.Concat(array.Select(item => new BetDetail(
				TransId: item!["trans_id"]!.GetValue<long>(),
				VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
				SportType: item!["sport_type"]?.GetValue<int>() ?? 0,
				TicketStatus: item!["ticket_status"]!.GetValue<string>(),
				TicketExtraStatus: item["ticket_extra_status"]?.ToString(),
				Stake: item!["stake"]!.GetValue<decimal>(),
				WinlostAmount: item!["winlost_amount"]!.GetValue<decimal>(),
				BuybackAmount: item!["buyback_amount"]?.GetValue<decimal>(),
				TransactionTime: (item!["transaction_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				SettlementTime: (item!["settlement_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				WinlostDatetime: (item!["winlost_datetime"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				AdditionalData: item.ToJsonString())));
		}

		array = data["BetVirtualSportDetails"]?.AsArray();
		if (array is not null && array.Count != 0)
		{
			betDetails = betDetails.Concat(array.Select(item => new BetDetail(
				TransId: item!["trans_id"]!.GetValue<long>(),
				VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
				SportType: item!["sport_type"]?.GetValue<int>() ?? 0,
				TicketStatus: item!["ticket_status"]!.GetValue<string>(),
				TicketExtraStatus: item["ticket_extra_status"]?.ToString(),
				Stake: item!["stake"]!.GetValue<decimal>(),
				WinlostAmount: item!["winlost_amount"]!.GetValue<decimal>(),
				BuybackAmount: item!["buyback_amount"]?.GetValue<decimal>(),
				TransactionTime: (item!["transaction_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				SettlementTime: (item!["settlement_time"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				WinlostDatetime: (item!["winlost_datetime"]?.ToString() ?? string.Empty).StringConvertToDatetime(),
				AdditionalData: item.ToJsonString())));
		}

		if (!betDetails.Any())
		{
			yield break;
		}

		foreach (var betDetail in betDetails)
		{
			yield return betDetail;
		}
	}

	public async ValueTask KickUserAsync(Uri apiUrl, string vendorId, string vendorMemberId, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["vendor_member_id"] = vendorMemberId
		};

		var url = new Uri(apiUrl, "/api/KickUser/");

		_ = await PostAsync(
		   apiUrl: url,
		   parameters: parameters,
		   logPrefix: nameof(KickUserAsync),
		   cancellationToken: cancellationToken).ConfigureAwait(false);
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

	public async ValueTask<bool> CheckIsOnlineAsync(Uri apiUrl, string vendorId, string vendorMemberId, CancellationToken cancellationToken = default)
	{
		var parameters = new Dictionary<string, string>
		{
			["vendor_id"] = vendorId,
			["vendor_member_id"] = vendorMemberId
		};

		var url = new Uri(apiUrl, "/api/CheckIsOnline/");

		var jsonNode = await PostAsync(
		   apiUrl: url,
		   parameters: parameters,
		   logPrefix: nameof(CheckIsOnlineAsync),
		   cancellationToken: cancellationToken).ConfigureAwait(false);

		if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
		{
			throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
		}

		var data = jsonNode["Data"]?.AsArray().FirstOrDefault()
			?? throw new Exception("取得點數失敗");

		return data["is_online"]!.GetValue<bool>();
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

		var requestContent = await content.ReadAsStringAsync().ConfigureAwait(false);

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
