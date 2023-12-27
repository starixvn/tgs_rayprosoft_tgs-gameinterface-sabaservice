using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class FundInGetService(
    IGameApiClient client,
    IGameVendorMember vendorMember) : IGetService<FundInQuery, FundResult>
{
    public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = vendorMember.ConvertToGameProviderMember(query.OperatorId, query.VendorMemberId);
        var vendorTransId = $"{query.OperatorId}_{query.VendorTransId}";

        var result = await client.FundTransferAsync(
            apiUrl: query.ApiUrl,
            vendorId: query.VendorId,
            vendorMemberId: vendorMemberId,
            vendorTransId: vendorTransId,
            directionType: GameDirectionType.Withdraw,
            walletType: GameWalletType.Sportsbook,
            currency: query.Currency,
            amount: query.Amount,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new FundResult(
            Check: result.Check,
            IsOrderExist: result.IsOrderExist,
            VendorTransId: query.VendorTransId,
            ActualVendorTransId: vendorTransId,
            TransId: result.TransId,
            Balance: result.AfterAmount);
    }
}
