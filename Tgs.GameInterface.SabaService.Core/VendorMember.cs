using Microsoft.Extensions.Options;

namespace Tgs.GameInterface.SabaService.Core;

internal class VendorMember : IVendorMember
{
    private readonly GameInterfaceSettingsOptions _gameInterfaceSettings;

    public VendorMember(IOptions<GameInterfaceSettingsOptions> options)
    {
        _gameInterfaceSettings = options.Value;
    }

    public string ConvertToGameProviderMember(string operatorId, string vendorMemberId)
        => _gameInterfaceSettings.ReleaseNamespace.Equals("prod", StringComparison.OrdinalIgnoreCase)
        ? string.Format("{0}_{1}", operatorId, vendorMemberId)
        : string.Format("{0}_{1}_test", operatorId, vendorMemberId);

    public string ConvertToPlatformMember(string operatorId, string vendorMemberId)
        => vendorMemberId.Replace(string.Format("{0}_", operatorId), string.Empty).Replace("_test", string.Empty);
}
