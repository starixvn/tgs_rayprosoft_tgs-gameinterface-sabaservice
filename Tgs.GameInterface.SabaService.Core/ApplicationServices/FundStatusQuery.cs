namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record FundStatusQuery(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	string OrderId);
