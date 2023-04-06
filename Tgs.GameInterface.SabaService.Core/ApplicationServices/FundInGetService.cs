using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class FundInGetService : IGetService<FundInQuery, FundResult>
{
    private readonly IGameApiClient _client;
    private readonly IGameVendorMember _vendorMember;

    public FundInGetService(
        IGameApiClient client,
        IGameVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async ValueTask<FundResult> GetAsync(FundInQuery query, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = _vendorMember.ConvertToGameProviderMember(query.OperatorId, query.VendorMemberId);
        var vendorTransId = $"{query.OperatorId}_{query.VendorTransId}";

        var result = await _client.FundTransferAsync(
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
