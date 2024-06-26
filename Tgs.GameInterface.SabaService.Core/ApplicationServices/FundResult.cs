namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record FundResult(
	bool Check,
	bool IsOrderExist,
	string OrderId,
	string VendorTransId,
	long TransId,
	string Resp);
