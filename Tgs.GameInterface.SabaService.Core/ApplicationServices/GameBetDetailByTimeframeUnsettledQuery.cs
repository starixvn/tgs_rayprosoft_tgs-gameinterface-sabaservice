namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameBetDetailByTimeframeUnsettledQuery(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTime StartTime,
    DateTime EndTime);
