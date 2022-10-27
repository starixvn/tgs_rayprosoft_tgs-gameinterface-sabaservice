namespace Tgs.GameInterface.SabaService.Core.Models;

public record CheckFundTransferResult
{
    public bool Retry { get; init; }
    public bool Success { get; init; }
}
