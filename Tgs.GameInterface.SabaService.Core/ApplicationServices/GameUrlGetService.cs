using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class GameUrlGetService : IGetService<GameUrlQuery, GameUrlResult>
{
    private readonly IGameApiClient _client;

    public GameUrlGetService(IGameApiClient client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async ValueTask<GameUrlResult> GetAsync(GameUrlQuery query, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = $"{query.OperatorId}_{query.VendorMemberId}";

        var result = await _client
            .GetSabaUrlAsync(
                apiUrl: query.ApiUrl,
                vendorId: query.VendorId,
                vendorMemberId: vendorMemberId,
                platformType: (query.IsMobile) ? GamePlatformType.Desktop : GamePlatformType.Mobile_H5,
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return new GameUrlResult(
            IsPlayerExist: result.IsPlayerExist,
            Url: result.Url);
    }
}
