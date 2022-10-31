namespace Tgs.GameInterface.SabaService.ViewModels;

public record GameHistoryResultViewModel(
    long LastVersionKey,
    IEnumerable<GameHistoryDetailViewModel> Details);
