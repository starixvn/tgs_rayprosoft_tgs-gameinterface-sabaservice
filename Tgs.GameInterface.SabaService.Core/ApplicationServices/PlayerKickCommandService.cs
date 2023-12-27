using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerKickCommandService(
    IGameApiClient client,
    IGameVendorMember vendorMember) : ICommandService<PlayerKickCommand>
{
    public async ValueTask ExecuteAsync(PlayerKickCommand command, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = vendorMember.ConvertToGameProviderMember(command.OperatorId, command.VendorMemberId);

        await client
             .KickUserAsync(
                 apiUrl: command.ApiUrl,
                 vendorId: command.VendorId,
                 vendorMemberId: vendorMemberId,
                 cancellationToken: cancellationToken)
             .ConfigureAwait(false);
    }
}
