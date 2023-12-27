namespace Tgs.GameInterface.SabaService.Core.Exceptions;

public class GameProviderResponseErrorException(string? errorCode, string? message) : Exception(
   $"The game manufacturer responds with an error code({errorCode}), please contact the engineer or game supplier for assistance.({message})")
{
}
