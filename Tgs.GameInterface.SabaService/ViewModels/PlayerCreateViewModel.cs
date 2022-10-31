using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.SabaService.ViewModels;

public record PlayerCreateViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string OperatorId { get; init; } = default!;
    [Required]
    public string VendorId { get; init; } = default!;
    [Required]
    public string BrandCode { get; init; } = default!;
    [Required]
    public string VendorMemberId { get; init; } = default!;
    [Required]
    public int OddsType { get; init; } = default!;
    [Required]
    public int Currency { get; init; } = default!;
}
