namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameHistorySettledByTimeframeQuery(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	DateTime StartTime,
	DateTime EndTime);
