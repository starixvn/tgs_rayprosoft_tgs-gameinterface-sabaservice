namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record PlayerBalanceResult(
    bool IsPlayerExist,
    decimal Balance);
