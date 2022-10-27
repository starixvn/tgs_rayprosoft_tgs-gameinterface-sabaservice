namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal record GameBetDetailResult(
         long LastVersionKey,
         IEnumerable<GameBetDetail> BetDetails);
