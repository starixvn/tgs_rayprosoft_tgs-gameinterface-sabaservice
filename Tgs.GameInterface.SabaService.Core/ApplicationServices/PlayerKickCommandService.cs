using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerKickCommandService : ICommandService<PlayerKickCommand>
{
    private readonly IGameApiClient _client;
    private readonly IGameVendorMember _vendorMember;

    public PlayerKickCommandService(
        IGameApiClient client,
        IGameVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public async ValueTask ExecuteAsync(PlayerKickCommand command, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = _vendorMember.ConvertToGameProviderMember(command.OperatorId, command.VendorMemberId);

        await _client
             .KickUserAsync(
                 apiUrl: command.ApiUrl,
                 vendorId: command.VendorId,
                 vendorMemberId: vendorMemberId,
                 cancellationToken: cancellationToken)
             .ConfigureAwait(false);
    }
}
