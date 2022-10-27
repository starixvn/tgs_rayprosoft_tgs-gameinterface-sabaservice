namespace Tgs.GameInterface.SabaService.Core.Models;

public record GetBetDetailResult
{
    public long LastVersionKey { get; init; }
    public IEnumerable<BetDetail> BetDetails { get; init; } = default!;
}
