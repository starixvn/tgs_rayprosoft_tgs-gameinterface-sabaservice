namespace Tgs.GameInterface.SabaService.Core.Models;

public record CheckFundTransferResult(
    bool Retry,
    bool IsOrderExist);
