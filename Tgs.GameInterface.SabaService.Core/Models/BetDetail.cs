namespace Tgs.GameInterface.SabaService.Core.Models;

public record BetDetail(
    long TransId,
    string VendorMemberId,
    int SportType,
    string TicketStatus,
    string? TicketExtraStatus,
    decimal Stake,
    decimal WinlostAmount,
    decimal? BuybackAmount,
    DateTime TransactionTime,
    DateTime SettlementTime,
    DateTime WinlostDatetime,
    string AdditionalData);
