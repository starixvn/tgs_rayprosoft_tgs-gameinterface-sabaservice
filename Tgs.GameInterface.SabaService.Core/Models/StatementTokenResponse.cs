using System.Text.Json.Serialization;

namespace Tgs.GameInterface.SabaService.Core.Models;

public record StatementTokenResponse
{
	[JsonPropertyName("access_token")]
	public required string AccessToken { get; set; }

	[JsonPropertyName("token_type")]
	public string? TokenType { get; set; }

	[JsonPropertyName("expires_in")]
	public required int ExpiresIn { get; set; }
}
