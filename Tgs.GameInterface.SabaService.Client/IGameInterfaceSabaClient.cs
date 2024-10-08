using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameInterface.SabaService.Client;

public interface IGameInterfaceSabaClient
{
	ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);

	ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, bool isMobile, string brandCode, CancellationToken cancellationToken = default);

	ValueTask<StatementReportTokenReply> StatementReportTokenAsync(Uri apiUrl, string username, string password, CancellationToken cancellationToken = default);

	ValueTask PlayerCreateAsync(Uri apiUrl, string operatorId, string vendorId, string brandCode, string playerCode, int oddsType, int currency, CancellationToken cancellationToken = default);
}
