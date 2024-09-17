using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Controllers;

[Route("saba")]
[ApiController]
public class SabaController : ControllerBase
{
	[HttpPost("report/statementurl", Name = "StatementUrl")]
	public ValueTask<StatementReportResult> StatementReportAsync(
	[FromServices] IGetService<StatementReportQuery, StatementReportResult> service,
	StatementReportQuery source)
	=> service.GetAsync(
		query: source,
		cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Funds the in asynchronous.
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
	///             "orderId": "49ED28A987624CA0A3F834C3B8A69AA2",
	///             "currency": 20,
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("fund/in", Name = "FundIn")]
	public ValueTask<FundResult> FundInAsync(
		[FromServices] IGetService<FundInQuery, FundResult> service,
		FundInQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Funds the out asynchronous.
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
	///             "orderId": "49ED28A987624CA0A3F834C3B8A69AA1",
	///             "currency": 20,
	///             "amount": 1
	///         }
	/// </remarks>
	[HttpPost("fund/out", Name = "FundOut")]
	public ValueTask<FundResult> FundOutAsync(
		[FromServices] IGetService<FundOutQuery, FundResult> service,
		FundOutQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Funds the status asynchronous.
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
	///             "orderId": "49ED28A987624CA0A3F834C3B8A69AA1"
	///         }
	/// </remarks>
	[HttpPost("fund/Status", Name = "FundStatus")]
	public ValueTask<FundStatusResult> FundStatusAsync(
		[FromServices] IGetService<FundStatusQuery, FundStatusResult> service,
		FundStatusQuery source)
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
	/// Games the history asynchronous.
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
	///             "versionKey": 639998
	///         }
	/// </remarks>
	[HttpPost("game/history", Name = "GameHistory")]
	public ValueTask<GameHistoryResult> GameHistoryAsync(
		[FromServices] IGetService<GameHistoryQuery, GameHistoryResult> service,
		GameHistoryQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the history settled by timeframe asynchronous.
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
	///             "startTime": "2022-10-27T15:50:00+08:00",
	///             "endTime": "2022-10-27T15:55:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("game/history/Settled", Name = "GameHistorySettledByTimeframe")]
	public IAsyncEnumerable<GameHistoryDetail> GameHistorySettledByTimeframeAsync(
		[FromServices] IQueryService<GameHistorySettledByTimeframeQuery, GameHistoryDetail> service,
		GameHistorySettledByTimeframeQuery source)
		=> service.QueryAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Games the history unsettled by timeframe asynchronous.
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
	///             "startTime": "2022-10-27T13:50:00+08:00",
	///             "endTime": "2022-10-27T13:55:00+08:00"
	///         }
	/// </remarks>
	[HttpPost("game/history/unsettled", Name = "GameHistoryUnsettledByTimeframe")]
	public IAsyncEnumerable<GameHistoryDetail> GameHistoryUnsettledByTimeframeAsync(
		[FromServices] IQueryService<GameHistoryUnsettledByTimeframeQuery, GameHistoryDetail> service,
		GameHistoryUnsettledByTimeframeQuery source)
		=> service.QueryAsync(
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

	/// <summary>
	/// Players the balance asynchronous.
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
	///             "playerCode": "4ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("player/balance", Name = "PlayerBalance")]
	public ValueTask<PlayerBalanceResult> PlayerBalanceAsync(
		[FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
		PlayerBalanceQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the logout asynchronous.
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
	///             "playerCode": "4ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("player/logout", Name = "PlayerLogout")]
	public ValueTask PlayerLogoutAsync(
		[FromServices] ICommandService<PlayerLogoutCommand> service,
		PlayerLogoutCommand source)
		=> service.ExecuteAsync(
			command: source,
			cancellationToken: HttpContext.RequestAborted);

	/// <summary>
	/// Players the online asynchronous.
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
	///             "playerCode": "4ucmqplxb029"
	///         }
	/// </remarks>
	[HttpPost("player/online", Name = "PlayerOnline")]
	public ValueTask<bool> PlayerOnlineAsync(
		[FromServices] IGetService<PlayerOnlineQuery, bool> service,
		PlayerOnlineQuery source)
		=> service.GetAsync(
			query: source,
			cancellationToken: HttpContext.RequestAborted);
}
