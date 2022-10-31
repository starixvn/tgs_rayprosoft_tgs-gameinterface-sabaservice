namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record PlayerKickCommand(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    string VendorMemberId);
