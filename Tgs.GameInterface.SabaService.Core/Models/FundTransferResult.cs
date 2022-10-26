using System.Text.Json.Serialization;

namespace Tgs.GameInterface.SabaService.Core.Models;

public record FundTransferResult
{
    [JsonPropertyName("trans_id")]
    public string TransId { get; init; } = default!;
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("before_amount")]
    public decimal? BeforeAmount { get; init; }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("after_amount")]
    public decimal? AfterAmount { get; init; }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("bonus_before_amount")]
    public decimal BonusBeforeAmount { get; init; }
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("bonus_after_amount")]
    public decimal BonusAfterAmount { get; init; }
    [JsonPropertyName("system_id")]
    public string System_id { get; init; } = default!;
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    [JsonPropertyName("status")]
    public int Status { get; init; }
}
