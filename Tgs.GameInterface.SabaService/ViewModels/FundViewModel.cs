using System.ComponentModel.DataAnnotations;

namespace Tgs.GameInterface.SabaService.ViewModels;

public record FundViewModel
{
    [Required]
    public Uri ApiUrl { get; init; } = default!;
    [Required]
    public string OperatorId { get; init; } = default!;
    [Required]
    public string VendorId { get; init; } = default!;
    [Required]
    public string VendorMemberId { get; init; } = default!;
    [Required]
    public string VendorTransId { get; init; } = default!;
    [Required]
    public int Currency { get; init; }
    [Required]
    [Range(minimum: 0, maximum: int.MaxValue)]
    public decimal Amount { get; init; }
}
