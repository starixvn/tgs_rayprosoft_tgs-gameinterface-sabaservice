namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record FundResult(
	string OrderId,
	string VendorTransId,
	string ActualVendorTransId,
	long TransId,
	bool Check,
	bool IsOrderExist,
	decimal Balance,
	string Resp);
