using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SabaService.Core.Exceptions;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

internal class GameApiClient : IGameApiClient
{
    private readonly HttpClient _client;
    private readonly ILogger<GameApiClient> _logger;

    public GameApiClient(
        HttpClient client,
        ILogger<GameApiClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async ValueTask<CheckFundTransferResult> CheckFundTransferAsync(
        Uri apiUrl,
        string vendorId,
        string vendorTransId,
        GameWalletType walletType,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["vendor_id"] = vendorId,
            ["vendor_trans_id"] = vendorTransId,
            ["wallet_id"] = ((int)walletType).ToString()
        };

        var url = new Uri(apiUrl, "/api/CheckFundTransfer/");

        var jsonNode = await PostAsync(
                url,
                parameters,
                "CheckFundTransfer",
                cancellationToken)
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

    public async ValueTask<GetBalanceResult> CheckUserBalanceAsync(
        Uri apiUrl,
        string vendorId,
        string vendorMemberId,
        GameWalletType walletType,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["vendor_id"] = vendorId,
            ["vendor_member_ids"] = vendorMemberId,
            ["wallet_id"] = ((int)walletType).ToString()
        };

        var url = new Uri(apiUrl, "/api/CheckUserBalance/");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "CheckUserBalance",
            cancellationToken).ConfigureAwait(false);

        if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
        {
            throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
        }

        var data = jsonNode["Data"]?.AsArray().FirstOrDefault()
            ?? throw new Exception("取得點數失敗");

        if (data["error_code"]?.GetValue<int>() is null or not 0)
        {
            return (data["error_code"]?.GetValue<int>() is null or not 2)
                ? throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString())
                : new GetBalanceResult(false, 0);
        }

        var balance = data["balance"]?.GetValue<decimal>()
             ?? throw new Exception("取得點數失敗");

        return new GetBalanceResult(true, balance);
    }

    public async ValueTask CreateMemberAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        string firstName,
        string lastName,
        int oddsType,
        int currency,
        decimal maxTransfer = 9999999999,
        decimal minTransfer = 0,
        CancellationToken cancellationToken = default)
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
            url,
            parameters,
            "CreateMember",
            cancellationToken).ConfigureAwait(false);

        if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
        {
            if (jsonNode["error_code"]?.GetValue<int>() is null or not 2 and not 6)
            {
                throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
            }
        }
    }

    public async ValueTask<FundTransferResult> FundTransferAsync(
        Uri apiUrl,
        string vendorId,
        string vendorMemberId,
        string vendorTransId,
        GameDirectionType directionType,
        GameWalletType walletType,
        int currency,
        decimal amount,
        CancellationToken cancellationToken = default)
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
            url,
            parameters,
            "FundTransfer",
            cancellationToken).ConfigureAwait(false);

        if (jsonNode["error_code"]?.GetValue<int>() is null or not 0 and not 1)
        {
            throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
        }

        if (jsonNode["error_code"]?.GetValue<int>() is 1)
        {
            return new FundTransferResult(
                 Check: true,
                 IsOrderExist: false);
        }

        var data = jsonNode["Data"]
            ?? throw new Exception("取得資料失敗");

        return new FundTransferResult(
            Check: false,
            IsOrderExist: data["status"]!.GetValue<int>() == 0,
            TransId: data["trans_id"]!.GetValue<long>(),
            BeforeAmount: data["before_amount"]!.GetValue<decimal>(),
            AfterAmount: data["after_amount"]!.GetValue<decimal>(),
            BonusBeforeAmount: data["bonus_before_amount"]!.GetValue<decimal>(),
            BonusAfterAmount: data["bonus_after_amount"]!.GetValue<decimal>(),
            SystemId: data["system_id"]!.GetValue<string>());
    }

    public async ValueTask<GetBetDetailResult> GetBetDetailAsync(
        Uri apiUrl,
        string vendorId,
        long versionKey,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["vendor_id"] = vendorId,
            ["version_key"] = versionKey.ToString(),
        };

        var url = new Uri(apiUrl, "/api/GetBetDetail/");

        var jsonNode = await PostAsync(
            url,
            parameters,
            "GetBetDetail",
            cancellationToken).ConfigureAwait(false);

        if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
        {
            throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
        }

        var data = jsonNode["Data"]
            ?? throw new Exception("取得Data為null");

        var lastVersionKey = data["last_version_key"]?.GetValue<long>()
            ?? throw new Exception("取得last_version_key為空值");

        IEnumerable<BetDetail> betDetails = new List<BetDetail>();

        if (data["BetDetails"] is not null && (data["BetDetails"]?.AsArray().Any() ?? false))
        {
            var array = data["BetDetails"]!.AsArray();

            betDetails = betDetails.Concat(array.Select(item => new BetDetail(
                TransId: item!["trans_id"]!.GetValue<long>(),
                VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
                SportType: item!["sport_type"]!.GetValue<int>(),
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

        if (data["BetNumberDetails"] is not null && (data["BetNumberDetails"]?.AsArray().Any() ?? false))
        {
            var array = data["BetNumberDetails"]!.AsArray();

            betDetails = betDetails.Concat(array.Select(item => new BetDetail(
                TransId: item!["trans_id"]!.GetValue<long>(),
                VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
                SportType: item!["sport_type"]!.GetValue<int>(),
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

        if (data["BetVirtualSportDetails"] is not null && (data["BetVirtualSportDetails"]?.AsArray().Any() ?? false))
        {
            var array = data["BetVirtualSportDetails"]!.AsArray();

            betDetails = betDetails.Concat(array.Select(item => new BetDetail(
                TransId: item!["trans_id"]!.GetValue<long>(),
                VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
                SportType: item!["sport_type"]!.GetValue<int>(),
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

    public async IAsyncEnumerable<BetDetail> GetBetDetailByTimeframeAsync(
        Uri apiUrl,
        string vendorId,
        GameTimeType timeType,
        DateTime startTime,
        DateTime endTime,
       [EnumeratorCancellation] CancellationToken cancellationToken = default)
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
            url,
            parameters,
            "GetBetDetailByTimeframe",
            cancellationToken).ConfigureAwait(false);

        if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
        {
            throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
        }

        var data = jsonNode["Data"];

        if (data is null)
        {
            yield break;
        }

        IEnumerable<BetDetail> betDetails = new List<BetDetail>();

        if (data["BetDetails"] is not null && (data["BetDetails"]?.AsArray().Any() ?? false))
        {
            var array = data["BetDetails"]!.AsArray();

            betDetails = betDetails.Concat(array.Select(item => new BetDetail(
                TransId: item!["trans_id"]!.GetValue<long>(),
                VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
                SportType: item!["sport_type"]!.GetValue<int>(),
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

        if (data["BetNumberDetails"] is not null && (data["BetNumberDetails"]?.AsArray().Any() ?? false))
        {
            var array = data["BetNumberDetails"]!.AsArray();

            betDetails = betDetails.Concat(array.Select(item => new BetDetail(
                TransId: item!["trans_id"]!.GetValue<long>(),
                VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
                SportType: item!["sport_type"]!.GetValue<int>(),
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

        if (data["BetVirtualSportDetails"] is not null && (data["BetVirtualSportDetails"]?.AsArray().Any() ?? false))
        {
            var array = data["BetVirtualSportDetails"]!.AsArray();

            betDetails = betDetails.Concat(array.Select(item => new BetDetail(
                TransId: item!["trans_id"]!.GetValue<long>(),
                VendorMemberId: item!["vendor_member_id"]!.GetValue<string>(),
                SportType: item!["sport_type"]!.GetValue<int>(),
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

    public async ValueTask KickUserAsync(
        Uri apiUrl,
        string vendorId,
        string vendorMemberId,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["vendor_id"] = vendorId,
            ["vendor_member_id"] = vendorMemberId
        };

        var url = new Uri(apiUrl, "/api/KickUser/");

        _ = await PostAsync(
           url,
           parameters,
           "KickUser",
           cancellationToken).ConfigureAwait(false);
    }

    public async ValueTask<GetGameUrlResult> GetSabaUrlAsync(
        Uri apiUrl,
        string vendorId,
        string vendorMemberId,
        GamePlatformType platformType,
        CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, string>
        {
            ["vendor_id"] = vendorId,
            ["platform"] = ((int)platformType).ToString(),
            ["vendor_member_id"] = vendorMemberId
        };

        var url = new Uri(apiUrl, "/api/GetSabaUrl/");

        var jsonNode = await PostAsync(
           url,
           parameters,
           "GetSabaUrl",
           cancellationToken).ConfigureAwait(false);

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

    private async ValueTask<JsonNode> PostAsync(
        Uri apiUrl,
        IEnumerable<KeyValuePair<string, string>> parameters,
        string logPrefix,
        CancellationToken cancellationToken = default)
    {
        logPrefix = $"{nameof(GameApiClient)}.{logPrefix}".ToLower();

        var sw = Stopwatch.StartNew();

        HttpContent content = new FormUrlEncodedContent(parameters);

        var httpResponseMessage = await _client.PostAsync(apiUrl, content, cancellationToken);

        sw.Stop();

        var responseMessage = default(string);

        using (var sr = new StreamReader(httpResponseMessage.Content.ReadAsStreamAsync(cancellationToken).Result))
        {
            responseMessage = await sr.ReadToEndAsync().ConfigureAwait(false) ?? string.Empty;
        }

        var apiLogEvent = new GameApiLogEvent
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

        _logger.LogInformation("{apiLogEvent}", JsonSerializer.Serialize(apiLogEvent));

        _ = httpResponseMessage.EnsureSuccessStatusCode();

        return JsonNode.Parse(responseMessage)!;
    }
}
