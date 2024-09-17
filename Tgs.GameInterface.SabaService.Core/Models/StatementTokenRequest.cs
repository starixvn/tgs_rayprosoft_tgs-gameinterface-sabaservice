namespace Tgs.GameInterface.SabaService.Core.Models;

public record StatementTokenRequest
{
	public required string Username { get; set; }

	public required string Password { get; set; }
}
