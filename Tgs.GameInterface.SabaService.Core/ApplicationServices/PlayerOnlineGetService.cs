using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerOnlineGetService(
	IGameApiClient client,
	IGameVendorMember vendorMember) : IGetService<PlayerOnlineQuery, bool>
{
	public ValueTask<bool> GetAsync(PlayerOnlineQuery query, CancellationToken cancellationToken = default)
	{
		var vendorMemberId = vendorMember.ConvertToGameProviderMember(query.OperatorId, query.PlayerCode);

		return client.CheckIsOnlineAsync(
			apiUrl: query.ApiUrl,
			vendorId: query.VendorId,
			vendorMemberId: vendorMemberId,
			cancellationToken: cancellationToken);
	}
}
