using System.Diagnostics;
using System.Net.Mime;
using System.Text;
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
    private readonly JsonSerializerOptions _jsonSerializerOptions;

    public GameApiClient(
        HttpClient client,
        ILogger<GameApiClient> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    public async ValueTask<bool> CheckFundTransferAsync(
        Uri apiUrl,
        string vendorId,
        string vendorTransId,
        GameWalletType walletType,
        CancellationToken cancellationToken = default)
    {
        JsonObject request = new JsonObject
        {
            { "vendor_id",vendorId },
            { "vendor_trans_id", vendorTransId },
             { "wallet_id ", (int)walletType }
        };

        var url = new Uri(apiUrl, "/api/CheckFundTransfer/");

        var jsonNode = await PostAsync(
                url,
                request,
                "CheckFundTransfer",
                cancellationToken)
            .ConfigureAwait(false);

        if (jsonNode["error_code"]?.GetValue<int>() is null or not 0)
        {
            throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString());
        }

        var data = jsonNode["Data"]
            ?? throw new Exception("取得資料失敗");

        var status = data["status"]?.GetValue<int>()
            ?? throw new Exception("取得狀態失敗");

        return status is 0
            || (status is 1
            ? false
            : throw new Exception("狀態未明，請繼續調用 \"checkfundtransfer\". I.如果沒有明確的回覆(statuscode 0 or 1)，請五分鐘後繼續調用直到有確切的回覆。"));
    }

    public async ValueTask<GetBalanceResult> CheckUserBalanceAsync(
        Uri apiUrl,
        string vendorId,
        string vendorMemberId,
        GameWalletType walletType,
        CancellationToken cancellationToken = default)
    {
        JsonObject request = new JsonObject
        {
            { "vendor_id",vendorId },
            { "vendor_member_ids", vendorMemberId },
            { "wallet_id",(int)walletType}
        };

        var url = new Uri(apiUrl, "/api/CheckUserBalance");

        var jsonNode = await PostAsync(
            url,
            request,
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
        string vendorId,
        string vendorMemberId,
        string operatorId,
        string firstName,
        string lastName,
        int oddsType,
        int currency,
        decimal maxTransfer = 9999999999,
        decimal minTransfer = 0,
        CancellationToken cancellationToken = default)
    {
        JsonObject request = new JsonObject
        {
            { "vendor_id",vendorId },
            { "vendor_member_ids", vendorMemberId },
            { "operatorId",operatorId},
            { "firstName",firstName},
            { "lastName",lastName},
            { "userName",vendorMemberId},
            { "oddsType ",oddsType},
            { "currency ",currency},
            { "maxTransfer ",maxTransfer},
            { "minTransfer ",minTransfer}
        };

        var url = new Uri(apiUrl, "/api/CreateMember/");

        var jsonNode = await PostAsync(
            url,
            request,
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
        JsonObject request = new JsonObject
        {
            { "vendor_id",vendorId },
            { "vendor_member_id ", vendorMemberId },
            { "vendor_trans_id  ", vendorTransId },
            { "amount", amount  },
            { "currency", currency  },
            { "direction ", (int)directionType  },
            { "wallet_id",(int)walletType}
        };

        var url = new Uri(apiUrl, "/api/FundTransfer/");

        var jsonNode = await PostAsync(
            url,
            request,
            "FundTransfer",
            cancellationToken).ConfigureAwait(false);

        return jsonNode["error_code"]?.GetValue<int>() is null or not 0
            ? throw new GameProviderResponseErrorException(jsonNode["errorCode"]?.ToString(), jsonNode["message"]?.ToString())
            : jsonNode["Data"]?.GetValue<FundTransferResult>()
            ?? throw new Exception("取得點數失敗");
    }

    public ValueTask GetBetDetailAsync(
        Uri apiUrl,
        string vendorId,
        long versionKey,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public ValueTask GetBetDetailByTimeframeAsync(
        Uri apiUrl,
        string vendorId,
        int timeType,
        DateTime startDate,
        DateTime endDate,
        CancellationToken cancellationToken = default) => throw new NotImplementedException();

    public async ValueTask KickUserAsync(
        Uri apiUrl,
        string vendorId,
        string vendorMemberId,
        CancellationToken cancellationToken = default)
    {
        JsonObject request = new JsonObject
        {
            { "vendor_id",vendorId },
            { "vendor_member_ids", vendorMemberId }
        };

        var url = new Uri(apiUrl, "/api/KickUser/");

        _ = await PostAsync(
           url,
           request,
           "KickUser",
           cancellationToken).ConfigureAwait(false);
    }

    private async ValueTask<JsonNode> PostAsync(
        Uri apiUrl,
        JsonObject request,
        string logPrefix,
        CancellationToken cancellationToken = default)
    {
        logPrefix = $"{nameof(GameApiClient)}.{logPrefix}".ToLower();

        var sw = Stopwatch.StartNew();

        var content = new StringContent(request.ToJsonString(), Encoding.UTF8, MediaTypeNames.Application.Json);
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
