namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record PlayerBalanceQuery(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    string VendorMemberId);
