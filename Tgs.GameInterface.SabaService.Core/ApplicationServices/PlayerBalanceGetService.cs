using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerBalanceGetService : IGetService<PlayerBalanceQuery, PlayerBalanceResult>
{
    private readonly IGameApiClient _client;
    private readonly IVendorMember _vendorMember;

    public PlayerBalanceGetService(
        IGameApiClient client,
        IVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async ValueTask<PlayerBalanceResult> GetAsync(PlayerBalanceQuery query, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = _vendorMember.ConvertToGameProviderMember(query.OperatorId, query.VendorMemberId);

        var result = await _client.CheckUserBalanceAsync(
            apiUrl: query.ApiUrl,
            vendorId: query.VendorId,
            vendorMemberId: vendorMemberId,
            walletType: GameWalletType.Sportsbook,
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return new PlayerBalanceResult(
             IsPlayerExist: result.IsPlayerExist,
             Balance: result.Balance);
    }
}
