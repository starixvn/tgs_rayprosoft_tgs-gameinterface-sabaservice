using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.GameInterface.SabaService.Core.Exceptions;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core
{
    internal class GameUrlApiClient : IGameUrlApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<GameUrlApiClient> _logger;

        public GameUrlApiClient(
            HttpClient client,
            ILogger<GameUrlApiClient> logger)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
                ["vendor_member_ids"] = vendorMemberId
            };

            var url = new Uri(apiUrl, "/api/GetSabaUrl");

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
}
