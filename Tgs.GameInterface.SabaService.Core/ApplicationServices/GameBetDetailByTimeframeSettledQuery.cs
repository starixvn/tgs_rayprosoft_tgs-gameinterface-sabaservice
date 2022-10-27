namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameBetDetailByTimeframeSettledQuery(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTime StartTime,
    DateTime EndTime);
