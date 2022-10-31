using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameInterface.SabaService.Client;

public interface IGameInterfaceSabaClient
{
    ValueTask<FundReply> FundInAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        string vendorTransId,
        int currency,
        decimal amount,
        CancellationToken cancellationToken = default);

    ValueTask<FundReply> FundOutAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        string vendorTransId,
        int currency,
        decimal amount,
        CancellationToken cancellationToken = default);

    ValueTask<FundStatusReply> FundStatusAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorTransId,
        CancellationToken cancellationToken = default);

    ValueTask<GameHistoryReply> GameHistoryAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        long versionKey,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<GameHistory> GameHistorySettledAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<GameHistory> GameHistoryUnsettledAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        DateTime startTime,
        DateTime endTime,
        CancellationToken cancellationToken = default);

    ValueTask<GameUrlReply> GameUrlAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        bool isMobile,
        CancellationToken cancellationToken = default);

    ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        CancellationToken cancellationToken = default);

    ValueTask PlayerCreateAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string brandCode,
        string vendorMemberId,
        int oddsType,
        int currency,
        CancellationToken cancellationToken = default);

    ValueTask PlayerKickAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        CancellationToken cancellationToken = default);
}
