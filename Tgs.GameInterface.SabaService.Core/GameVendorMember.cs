using Microsoft.Extensions.Options;

namespace Tgs.GameInterface.SabaService.Core;

internal class GameVendorMember(IOptions<GameInterfaceSettingsOptions> options) : IGameVendorMember
{
	private readonly GameInterfaceSettingsOptions _gameInterfaceSettings = options.Value;

	public string ConvertToPlatformMember(string operatorId, string vendorMemberId)
		=> vendorMemberId.Replace(string.Format("{0}_", operatorId), string.Empty).Replace("_test", string.Empty);

	public string ConvertToGameProviderMember(string brandCode, string vendorMemberId)
			=> _gameInterfaceSettings.ReleaseNamespace.Equals("prod", StringComparison.OrdinalIgnoreCase)
		? string.Format("{0}_{1}", brandCode, vendorMemberId)
		: string.Format("{0}_{1}_test", brandCode, vendorMemberId);
}
