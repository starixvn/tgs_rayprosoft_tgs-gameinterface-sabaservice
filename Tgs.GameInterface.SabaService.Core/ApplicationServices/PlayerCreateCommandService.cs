using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerCreateCommandService(
	IGameApiClient client,
	IGameVendorMember vendorMember) : ICommandService<PlayerCreateCommand>
{
	public ValueTask ExecuteAsync(PlayerCreateCommand command, CancellationToken cancellationToken = default)
	{
		var vendorMemberId = vendorMember.ConvertToGameProviderMember(command.BrandCode, command.PlayerCode);

		return client.CreateMemberAsync(
			apiUrl: command.ApiUrl,
			operatorId: command.OperatorId,
			vendorId: command.VendorId,
			vendorMemberId: vendorMemberId,
			oddsType: command.OddsType,
			currency: command.Currency,
			cancellationToken: cancellationToken);
	}
}
