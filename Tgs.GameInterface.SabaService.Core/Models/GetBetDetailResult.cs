namespace Tgs.GameInterface.SabaService.Core.Models;

public record GetBetDetailResult(
    long LastVersionKey,
    IEnumerable<BetDetail> Details);
