using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameUrlGetService(
	IGameApiClient client,
	IGameVendorMember vendorMember) : IGetService<GameUrlQuery, GameUrlResult>
{
	public async ValueTask<GameUrlResult> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
	{
		var vendorMemberId = vendorMember.ConvertToGameProviderMember(query.BrandCode, query.PlayerCode);

		var platformType = query.IsMobile ? GamePlatformType.Mobile_H5 : GamePlatformType.Desktop;

		var result = await client
			.GetSabaUrlAsync(
				apiUrl: query.ApiUrl,
				vendorId: query.VendorId,
				vendorMemberId: vendorMemberId,
				platformType: platformType,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new GameUrlResult(
			IsPlayerExist: result.IsPlayerExist,
			GameUrl: result.GameUrl);
	}
}
