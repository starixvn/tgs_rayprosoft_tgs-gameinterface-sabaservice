using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameInterface.SabaService.ViewModels;

public record GameHistoryDetailViewModel(
    string PlayerCode,
    string ExternalRoundId,
    bool IsCashout,
    bool IsClosed,
    GameHistoryStatus Status,
    DateTimeOffset ExternalCreatedOn,
    DateTimeOffset ExternalClosedOn,
    decimal Bet,
    decimal ValidBet,
    decimal PlayerWinLoss,
    decimal Turnover,
    decimal Settle,
    string AdditionalData);
