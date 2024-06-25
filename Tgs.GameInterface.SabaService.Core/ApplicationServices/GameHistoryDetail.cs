namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record GameHistoryDetail(
	string PlayerCode,
	string ExternalRoundId,
	bool IsCashout,
	bool IsClosed,
	string Status,
	DateTimeOffset ExternalCreatedOn,
	DateTimeOffset ExternalClosedOn,
	decimal Bet,
	decimal ValidBet,
	decimal PlayerWinLoss,
	decimal Turnover,
	decimal Settle,
	string AdditionalData);
