namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record FundInQuery(
    Uri ApiUrl,
    string OperatorId,
    string VendorId,
    string VendorMemberId,
    string VendorTransId,
    int Currency,
    decimal Amount);
