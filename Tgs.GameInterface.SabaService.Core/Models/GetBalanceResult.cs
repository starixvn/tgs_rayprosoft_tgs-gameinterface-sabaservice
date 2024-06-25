namespace Tgs.GameInterface.SabaService.Core.Models;

public record GetBalanceResult(
	bool IsPlayerExist,
	decimal Balance,
	decimal Outstanding);
