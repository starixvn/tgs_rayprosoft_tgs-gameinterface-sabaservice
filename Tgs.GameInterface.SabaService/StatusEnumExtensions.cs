using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameInterface.SabaService;

public static class StatusEnumExtensions
{
    public static GameHistoryStatus StringConvertToEnum(this string str)
        => str.ToLower() switch
        {
            "waiting" => GameHistoryStatus.Waiting,
            "running" => GameHistoryStatus.Running,
            "void" => GameHistoryStatus.Void,
            "refund" => GameHistoryStatus.Refund,
            "reject" => GameHistoryStatus.Reject,
            "lose" => GameHistoryStatus.Lose,
            "won" => GameHistoryStatus.Won,
            "draw" => GameHistoryStatus.Draw,
            "half won" => GameHistoryStatus.HalfWon,
            "half lose" => GameHistoryStatus.HalfLose,
            _ => GameHistoryStatus.Unknown
        };
}
