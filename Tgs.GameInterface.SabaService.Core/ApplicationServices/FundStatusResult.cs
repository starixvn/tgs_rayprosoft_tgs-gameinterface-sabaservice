namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record FundStatusResult(
    bool Retry,
    bool IsOrderExist);
