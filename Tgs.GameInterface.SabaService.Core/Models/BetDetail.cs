namespace Tgs.GameInterface.SabaService.Core.Models;

public record BetDetail
{
    public long TransId { get; init; }
    public string VendorMemberId { get; init; } = default!;

    public int SportType { get; init; } = default!;
    public GameTicketStatus TicketStatus { get; init; }
    public string? TicketExtraStatus { get; init; } = default!;
    public decimal Stake { get; init; }
    public decimal WinlostAmount { get; init; }
    public decimal? BuybackAmount { get; init; }
    public DateTime TransactionTime { get; init; }

    public DateTime? SettlementTime { get; init; }

    public DateTime? WinlostDatetime { get; init; }
    public string AdditionalData { get; init; } = default!;
}
