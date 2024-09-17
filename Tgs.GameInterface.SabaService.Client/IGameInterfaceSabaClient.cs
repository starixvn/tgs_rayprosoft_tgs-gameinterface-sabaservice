using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameInterface.SabaService.Client;

public interface IGameInterfaceSabaClient
{
	ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundInAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, string orderId, int currency, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<FundReply> FundOutAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, string orderId, int currency, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<FundStatusReply> FundStatusAsync(Uri apiUrl, string operatorId, string vendorId, string orderId, CancellationToken cancellationToken = default);

	ValueTask<GameHistoryReply> GameHistoryAsync(Uri apiUrl, string operatorId, string vendorId, long versionKey, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameHistory> GameHistorySettledAsync(Uri apiUrl, string operatorId, string vendorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

	IAsyncEnumerable<GameHistory> GameHistoryUnsettledAsync(Uri apiUrl, string operatorId, string vendorId, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

	ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, bool isMobile, CancellationToken cancellationToken = default);

	ValueTask<string> StatementReportUrlAsync(Uri apiUrl, long transactionId, long baseCurrency = 0, string language = null, string playerUsername = null, CancellationToken cancellationToken = default);

	ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, CancellationToken cancellationToken = default);

	ValueTask PlayerCreateAsync(Uri apiUrl, string operatorId, string vendorId, string brandCode, string playerCode, int oddsType, int currency, CancellationToken cancellationToken = default);

	ValueTask PlayerLogoutAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, CancellationToken cancellationToken = default);

	ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, CancellationToken cancellationToken = default);
}
