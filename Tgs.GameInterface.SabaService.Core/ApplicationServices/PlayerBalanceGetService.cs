using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerBalanceGetService(
	IGameApiClient client,
	IGameVendorMember vendorMember) : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
	public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
	{
		var vendorMemberId = vendorMember.ConvertToGameProviderMember(query.OperatorId, query.PlayerCode);

		var result = await client.CheckUserBalanceAsync(
			apiUrl: query.ApiUrl,
			vendorId: query.VendorId,
			vendorMemberId: vendorMemberId,
			walletType: GameWalletType.Sportsbook,
			cancellationToken: cancellationToken).ConfigureAwait(false);

		return new PlayerBalanceResult
		{
			IsPlayerExist = result.IsPlayerExist,
			FreeBalance = result.Balance,
			OutstandingBalance = result.Outstanding
		};
	}
}
