using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

public interface IGameUrlApiClient
{
    ValueTask<GetGameUrlResult> GetSabaUrlAsync(
        Uri apiUrl,
        string vendorId,
        string vendorMemberId,
        GamePlatformType platformType,
        CancellationToken cancellationToken = default);
}
