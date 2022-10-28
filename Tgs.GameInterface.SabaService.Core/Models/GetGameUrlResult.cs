namespace Tgs.GameInterface.SabaService.Core.Models;

public record GetGameUrlResult(
    bool IsPlayerExist,
    string GameUrl);
