using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerCreateCommandService : ICommandService<PlayerCreateCommand>
{
    private readonly IGameApiClient _client;
    private readonly IVendorMember _vendorMember;

    public PlayerCreateCommandService(
        IGameApiClient client,
        IVendorMember vendorMember)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = _vendorMember.ConvertToGameProviderMember(command.OperatorId, command.VendorMemberId);

        return _client.CreateMemberAsync(
            apiUrl: command.ApiUrl,
            operatorId: command.OperatorId,
            vendorId: command.VendorId,
            vendorMemberId: vendorMemberId,
            firstName: command.BrandCode,
            lastName: command.VendorMemberId,
            oddsType: command.OddsType,
            currency: command.Currency,
            cancellationToken: cancellationToken);
    }
}
