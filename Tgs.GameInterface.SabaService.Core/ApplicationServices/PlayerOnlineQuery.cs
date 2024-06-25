namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record PlayerOnlineQuery(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	string PlayerCode);
