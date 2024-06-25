namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record PlayerCreateCommand(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	string BrandCode,
	string PlayerCode,
	int OddsType,
	int Currency);
