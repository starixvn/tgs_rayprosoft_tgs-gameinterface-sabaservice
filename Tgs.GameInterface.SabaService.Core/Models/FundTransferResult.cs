namespace Tgs.GameInterface.SabaService.Core.Models;

public record FundTransferResult(
	bool Check,
	bool IsOrderExist,
	long TransId = 0,
	decimal BeforeAmount = 0,
	decimal AfterAmount = 0,
	decimal BonusBeforeAmount = 0,
	decimal BonusAfterAmount = 0,
	string SystemId = "");
