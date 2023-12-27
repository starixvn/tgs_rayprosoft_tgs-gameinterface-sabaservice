using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class FundStatusGetService(IGameApiClient client) : IGetService<FundStatusQuery, FundStatusResult>
{
    public async ValueTask<FundStatusResult> GetAsync(FundStatusQuery query, CancellationToken cancellationToken = default)
    {
        var vendorTransId = $"{query.OperatorId}_{query.VendorTransId}";

        var result = await client
            .CheckFundTransferAsync(
                apiUrl: query.ApiUrl,
                vendorId: query.VendorId,
                vendorTransId: vendorTransId,
                walletType: GameWalletType.Sportsbook,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new FundStatusResult(
            Retry: result.Retry,
            IsOrderExist: result.IsOrderExist);
    }
}
