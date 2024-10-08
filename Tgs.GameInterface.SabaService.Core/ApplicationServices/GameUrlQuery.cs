namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameUrlQuery(
	Uri ApiUrl,
	string VendorId,
	string PlayerCode,
	bool IsMobile,
	string BrandCode);
