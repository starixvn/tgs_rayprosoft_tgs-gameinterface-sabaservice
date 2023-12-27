using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerCreateCommandService(
    IGameApiClient client,
    IGameVendorMember vendorMember) : ICommandService<PlayerCreateCommand>
{
    public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
    {
        var vendorMemberId = vendorMember.ConvertToGameProviderMember(command.OperatorId, command.VendorMemberId);

        return client.CreateMemberAsync(
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
