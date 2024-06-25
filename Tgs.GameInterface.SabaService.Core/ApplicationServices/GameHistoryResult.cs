namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameHistoryResult(
	long LastVersionKey,
	IEnumerable<GameHistoryDetail> Details);
