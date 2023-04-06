using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameUrlGetService : IGetService<GameUrlQuery, GameUrlResult>
{
    private readonly IGameApiClient _client;
    private readonly IGameVendorMember _vendorMember;

    public GameUrlGetService(
        IGameApiClient client,
        IGameVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async ValueTask<GameUrlResult> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = _vendorMember.ConvertToGameProviderMember(query.OperatorId, query.VendorMemberId);

        var platformType = (query.IsMobile) ? GamePlatformType.Mobile_H5 : GamePlatformType.Desktop;

        var result = await _client
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
