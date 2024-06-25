using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

internal class PlayerLogoutCommandService(
	IGameApiClient client,
	IGameVendorMember vendorMember) : ICommandService<PlayerLogoutCommand>
{
	public async ValueTask ExecuteAsync(PlayerLogoutCommand command, CancellationToken cancellationToken = default)
	{
		var vendorMemberId = vendorMember.ConvertToGameProviderMember(command.OperatorId, command.PlayerCode);

		await client
			 .KickUserAsync(
				 apiUrl: command.ApiUrl,
				 vendorId: command.VendorId,
				 vendorMemberId: vendorMemberId,
				 cancellationToken: cancellationToken)
			 .ConfigureAwait(false);
	}
}
