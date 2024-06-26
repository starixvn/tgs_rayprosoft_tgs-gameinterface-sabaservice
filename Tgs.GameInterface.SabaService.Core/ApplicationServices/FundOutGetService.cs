using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class FundOutGetService(
	IGameApiClient client,
	IGameVendorMember vendorMember) : IGetService<FundOutQuery, FundResult>
{
	public async ValueTask<FundResult> GetAsync(FundOutQuery query, CancellationToken cancellationToken = default)
	{
		var vendorMemberId = vendorMember.ConvertToGameProviderMember(query.OperatorId, query.PlayerCode);
		var vendorTransId = $"{query.OperatorId}_{query.OrderId}";

		var result = await client.FundTransferAsync(
			apiUrl: query.ApiUrl,
			vendorId: query.VendorId,
			vendorMemberId: vendorMemberId,
			vendorTransId: vendorTransId,
			directionType: GameDirectionType.Deposit,
			walletType: GameWalletType.Sportsbook,
			currency: query.Currency,
			amount: query.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return new FundResult(
			Check: result.Check,
			IsOrderExist: result.IsOrderExist,
			OrderId: query.OrderId,
			VendorTransId: vendorTransId,
			TransId: result.TransId,
			Resp: result.Resp);
	}
}
