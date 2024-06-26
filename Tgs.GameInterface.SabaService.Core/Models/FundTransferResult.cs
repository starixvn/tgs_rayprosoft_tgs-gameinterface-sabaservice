namespace Tgs.GameInterface.SabaService.Core.Models;

public record FundTransferResult(
	bool Check,
	bool IsOrderExist,
	long TransId,
	string Resp);
