namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record StatementReportQuery()
{
	public required Uri ApiUrl { get; set; } = default!;

	public required long TransactionId { get; set; }

	public required string Username { get; set; } = default!;

	public required string Password { get; set; } = default!;

	public long? BaseCurrency { get; internal set; }

	public string? Language { get; internal set; }

	public string? PlayerUsername { get; internal set; }
}
