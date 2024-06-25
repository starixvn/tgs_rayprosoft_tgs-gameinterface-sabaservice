namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameHistoryQuery(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	long VersionKey);
