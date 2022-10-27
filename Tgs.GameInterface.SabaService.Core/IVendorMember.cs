namespace Tgs.GameInterface.SabaService.Core;

public interface IVendorMember
{
    string ConvertToGameProviderMember(
        string operatorId,
        string vendorMemberId);

    string ConvertToPlatformMember(
        string operatorId,
        string vendorMemberId);
}
