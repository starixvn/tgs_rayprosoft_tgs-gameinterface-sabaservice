using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Controllers;

[Route("saba")]
[ApiController]
public class SabaController : ControllerBase
{
	[HttpPost("report/statementtoken", Name = "StatementToken")]
	public ValueTask<StatementTokenResult> StatementReportTokenAsync(
	[FromServices] IGetService<StatementReportTokenQuery, StatementTokenResult> service,
	StatementReportTokenQuery source)
	=> service.GetAsync(
		query: source,
		cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the URL by sports book asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://h0i2tsa.bw6688.com/",
	///             "operatorId": "tg",
	///             "vendorId": "um5vspnzkx",
	///             "playerCode": "4ucmqplxb029",
	///             "isMobile": false
	///         }
	/// </remarks>
	[HttpPost("game/url", Name = "GameUrl")]
	public ValueTask<GameUrlResult> GameUrlAsync(
		[FromServices] IGetService<GameUrlQuery, GameUrlResult> service,
		GameUrlQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the create asynchronous.
	/// </summary>
	/// <param name="service">The service.</param>
	/// <param name="source">The source.</param>
	/// <returns></returns>
	/// <remarks>
	/// <b>Example</b><br />
	///
	///     Post
	///
	///         {
	///             "apiUrl": "http://h0i2tsa.bw6688.com/",
	///             "operatorId": "tg",
	///             "vendorId": "um5vspnzkx",
	///             "brandCode": "GPI",
	///             "playerCode": "4ucmqplxb029",
	///             "oddsType": 2,
	///             "currency": 20
	///         }
	/// </remarks>
	[HttpPost("player/create", Name = "PlayerCreate")]
	public ValueTask PlayerCreateAsync(
		[FromServices] ICommandService<PlayerCreateCommand> service,
		PlayerCreateCommand source)
		=> service.ExecuteAsync(
			command: source,
			cancellationToken: HttpContext.RequestAborted);
}
