using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

public interface IGameHistoryCalculate
{
    GameHistoryDetail Execute(string operatorId, BetDetail detail);
}
