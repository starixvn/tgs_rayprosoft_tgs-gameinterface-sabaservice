namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record PlayerLogoutCommand(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	string PlayerCode);
