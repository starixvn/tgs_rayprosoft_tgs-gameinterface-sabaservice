using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

public interface IGameApiClient
{
	ValueTask CreateMemberAsync(Uri apiUrl, string operatorId, string vendorId, string vendorMemberId, int oddsType, int currency, decimal maxTransfer = 9999999999, decimal minTransfer = 0, CancellationToken cancellationToken = default);

	ValueTask<GetGameUrlResult> GetSabaUrlAsync(Uri apiUrl, string vendorId, string vendorMemberId, GamePlatformType platformType, CancellationToken cancellationToken = default);

	ValueTask<StatementTokenResult> GetStatementReportToken(Uri apiUrl, string username, string password, CancellationToken cancellationToken = default);
}
