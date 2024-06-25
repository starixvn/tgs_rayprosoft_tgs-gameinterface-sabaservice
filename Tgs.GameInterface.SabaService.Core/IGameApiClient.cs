using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

public interface IGameApiClient
{
	ValueTask CreateMemberAsync(Uri apiUrl, string operatorId, string vendorId, string vendorMemberId, string firstName, string lastName, int oddsType, int currency, decimal maxTransfer = 9999999999, decimal minTransfer = 0, CancellationToken cancellationToken = default);

	ValueTask<FundTransferResult> FundTransferAsync(Uri apiUrl, string vendorId, string vendorMemberId, string vendorTransId, GameDirectionType directionType, GameWalletType walletType, int currency, decimal amount, CancellationToken cancellationToken = default);

	ValueTask<CheckFundTransferResult> CheckFundTransferAsync(Uri apiUrl, string vendorId, string vendorTransId, GameWalletType walletType, CancellationToken cancellationToken = default);

	ValueTask<GetBetDetailResult> GetBetDetailAsync(Uri apiUrl, string vendorId, long versionKey, CancellationToken cancellationToken = default);

	ValueTask KickUserAsync(Uri apiUrl, string vendorId, string vendorMemberId, CancellationToken cancellationToken = default);

	ValueTask<bool> CheckIsOnlineAsync(Uri apiUrl, string vendorId, string vendorMemberId, CancellationToken cancellationToken = default);

	ValueTask<GetBalanceResult> CheckUserBalanceAsync(Uri apiUrl, string vendorId, string vendorMemberId, GameWalletType walletType, CancellationToken cancellationToken = default);

	IAsyncEnumerable<BetDetail> GetBetDetailByTimeframeAsync(Uri apiUrl, string vendorId, GameTimeType timeType, DateTime startTime, DateTime endTime, CancellationToken cancellationToken = default);

	ValueTask<GetGameUrlResult> GetSabaUrlAsync(Uri apiUrl, string vendorId, string vendorMemberId, GamePlatformType platformType, CancellationToken cancellationToken = default);
}
