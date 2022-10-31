using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.SabaService.ViewModels;

public record GameHistoryByTimeframeViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string OperatorId { get; init; } = default!;
    [Required]
    public string VendorId { get; init; } = default!;
    [Required]
    public DateTime StartTime { get; init; } = default!;
    [Required]
    public DateTime EndTime { get; init; } = default!;
}
