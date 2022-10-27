namespace Tgs.GameInterface.SabaService.Core.Models;

public record FundTransferResult
{
    public long TransId { get; init; } = default!;
    public decimal? BeforeAmount { get; init; }
    public decimal? AfterAmount { get; init; }
    public decimal BonusBeforeAmount { get; init; }
    public decimal BonusAfterAmount { get; init; }
    public string SystemId { get; init; } = default!;
    public bool Check { get; init; }
    public bool Success { get; init; }
}
