namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameUrlQuery(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	string PlayerCode,
	bool IsMobile);
