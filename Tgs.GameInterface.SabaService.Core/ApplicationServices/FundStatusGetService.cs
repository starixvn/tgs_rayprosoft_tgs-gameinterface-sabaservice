using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class FundStatusGetService : IGetService<FundStatusQuery, FundStatusResult>
{
    private readonly IGameApiClient _client;

    public FundStatusGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<FundStatusResult> GetAsync(FundStatusQuery query, CancellationToken cancellationToken = default)
    {
        var vendorTransId = $"{query.OperatorId}_{query.VendorTransId}";

        var result = await _client
            .CheckFundTransferAsync(
                apiUrl: query.ApiUrl,
                vendorId: query.VendorId,
                vendorTransId: vendorTransId,
                walletType: GameWalletType.Sportsbook,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new FundStatusResult(
            Retry: result.Retry,
            Success: result.Success);
    }
}
