namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record PlayerCreateCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    string BrandCode,
    string VendorMemberId,
    int OddsType,
    int Currency);
