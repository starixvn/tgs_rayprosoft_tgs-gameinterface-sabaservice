using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.SabaService.ViewModels;

public record PlayerBalanceViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string OperatorId { get; init; } = default!;
    [Required]
    public string VendorId { get; init; } = default!;
    [Required]
    public string VendorMemberId { get; init; } = default!;
}
