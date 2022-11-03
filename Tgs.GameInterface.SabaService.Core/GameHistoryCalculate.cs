using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

internal class GameHistoryCalculate
{
    private readonly VendorMember _vendorMember;

    public GameHistoryCalculate(VendorMember vendorMember)
    {
        _vendorMember = vendorMember ?? throw new ArgumentNullException(nameof(vendorMember));
    }

    public GameHistoryDetail Process(string operatorId, BetDetail detail)
    {
        var playerCode = _vendorMember.ConvertToPlatformMember(operatorId, detail.VendorMemberId);
        var isClosed = !detail.TicketStatus.Equals("running", StringComparison.OrdinalIgnoreCase);
        var isReject = detail.TicketStatus.Equals("reject", StringComparison.OrdinalIgnoreCase);
        var externalClosedOn = DateTime.UnixEpoch;
        var bet = detail.Stake;
        var validBet = detail.Stake;
        var playerWinLoss = 0m;
        var turnover = 0m;
        var settle = 0m;
        var isCashout = detail.BuybackAmount.HasValue;

        if (isClosed)
        {
            validBet = isCashout ? 0 : !isReject ? detail.Stake : 0;
            playerWinLoss = detail.WinlostAmount;
            turnover = isCashout ? 0 : !isReject ? detail.Stake : 0;
            settle = detail.BuybackAmount ?? (!isReject ? detail.WinlostAmount + detail.Stake : 0);
            externalClosedOn = detail.SportType > 99
                ? detail.WinlostDatetime
                : detail.SettlementTime.ToUniversalTime() != DateTime.UnixEpoch
                ? detail.SettlementTime
                : detail.WinlostDatetime;
        }

        return new GameHistoryDetail(
            PlayerCode: playerCode,
            ExternalRoundId: detail.TransId.ToString(),
            IsCashout: isCashout,
            IsClosed: isClosed,
            Status: detail.TicketStatus,
            ExternalCreatedOn: detail.TransactionTime,
            ExternalClosedOn: externalClosedOn,
            Bet: bet,
            ValidBet: validBet,
            PlayerWinLoss: playerWinLoss,
            Turnover: turnover,
            Settle: settle,
            AdditionalData: detail.AdditionalData);
    }
}
