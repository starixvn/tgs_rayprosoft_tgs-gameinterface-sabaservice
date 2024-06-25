namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record FundOutQuery(
	Uri ApiUrl,
	string OperatorId,
	string VendorId,
	string PlayerCode,
	string OrderId,
	int Currency,
	decimal Amount);
