namespace Tgs.GameInterface.SabaService.Core.ApplicationServices;

public record StatementReportQuery()
{
	public required string PlayerCode { get; set; }

	public required long TransactionId { get; set; }

	public long? BaseCurrency { get; internal set; }

	public string? Language { get; internal set; }

	public string? PlayerUsername { get; internal set; }
}
