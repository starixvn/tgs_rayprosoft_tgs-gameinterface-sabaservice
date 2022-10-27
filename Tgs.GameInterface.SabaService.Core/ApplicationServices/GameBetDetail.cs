namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameBetDetail(
    string PlayerCode,
    string ExternalRoundId,
    bool IsClosed,
    GameTicketStatus Status,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal PlayerWinLoss,
    decimal Turnover,
    decimal Settle,
    string AdditionalData);
