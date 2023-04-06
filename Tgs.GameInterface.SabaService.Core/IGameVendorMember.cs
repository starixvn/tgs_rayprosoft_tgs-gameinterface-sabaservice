namespace Tgs.GameInterface.SabaService.Core;

public interface IGameVendorMember
{
    string ConvertToPlatformMember(string operatorId, string vendorMemberId);

    string ConvertToGameProviderMember(string operatorId, string vendorMemberId);
}
