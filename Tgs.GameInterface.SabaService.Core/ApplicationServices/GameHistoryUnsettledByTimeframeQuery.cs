namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameHistoryUnsettledByTimeframeQuery(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    DateTime StartTime,
    DateTime EndTime);
