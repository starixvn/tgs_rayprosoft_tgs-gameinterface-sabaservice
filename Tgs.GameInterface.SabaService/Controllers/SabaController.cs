using Microsoft.AspNetCore.Mvc;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.ViewModels;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.Controllers;

[Route("saba")]
[ApiController]
public class SabaController : ControllerBase
{
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
    ///             "vendorMemberId": "4ucmqplxb029",
    ///             "vendorTransId": "49ED28A987624CA0A3F834C3B8A69AA2",
    ///             "currency": 20,
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/in", Name = "FundIn")]
    public async ValueTask<FundResultViewModel> FundInAsync(
        [FromServices] IGetService<FundInQuery, FundResult> service,
        FundViewModel source
        )
    {
        var query = new FundInQuery(
            ApiUrl: source.ApiUrl,
            OperatorId: source.OperatorId,
            VendorId: source.VendorId,
            VendorMemberId: source.VendorMemberId,
            VendorTransId: source.VendorTransId,
            Currency: source.Currency,
            Amount: source.Amount);

        var result = await service
                 .GetAsync(
                     query: query,
                     cancellationToken: HttpContext.RequestAborted)
                 .ConfigureAwait(false);

        return new FundResultViewModel(
            Check: result.Check,
            IsOrderExist: result.IsOrderExist,
            VendorTransId: result.VendorTransId,
            ActualVendorTransId: result.ActualVendorTransId,
            TransId: result.TransId,
            Balance: result.Balance);
    }

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
    ///             "vendorMemberId": "4ucmqplxb029",
    ///             "vendorTransId": "49ED28A987624CA0A3F834C3B8A69AA1",
    ///             "currency": 20,
    ///             "amount": 1
    ///         }
    /// </remarks>
    [HttpPost("fund/out", Name = "FundOut")]
    public async ValueTask<FundResultViewModel> FundOutAsync(
        [FromServices] IGetService<FundOutQuery, FundResult> service,
        FundViewModel source
        )
    {
        var query = new FundOutQuery(
            ApiUrl: source.ApiUrl,
            OperatorId: source.OperatorId,
            VendorId: source.VendorId,
            VendorMemberId: source.VendorMemberId,
            VendorTransId: source.VendorTransId,
            Currency: source.Currency,
            Amount: source.Amount);

        var result = await service
                 .GetAsync(
                     query: query,
                     cancellationToken: HttpContext.RequestAborted)
                 .ConfigureAwait(false);

        return new FundResultViewModel(
            Check: result.Check,
            IsOrderExist: result.IsOrderExist,
            VendorTransId: result.VendorTransId,
            ActualVendorTransId: result.ActualVendorTransId,
            TransId: result.TransId,
            Balance: result.Balance);
    }

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
    ///             "vendorTransId": "49ED28A987624CA0A3F834C3B8A69AA1"
    ///         }
    /// </remarks>
    [HttpPost("fund/Status", Name = "FundStatus")]
    public async ValueTask<FundStatusResultViewModel> FundStatusAsync(
        [FromServices] IGetService<FundStatusQuery, FundStatusResult> service,
        FundStatusViewModel source
        )
    {
        var query = new FundStatusQuery(
            ApiUrl: source.ApiUrl,
            OperatorId: source.OperatorId,
            VendorId: source.VendorId,
            VendorTransId: source.VendorTransId);

        var result = await service
                 .GetAsync(
                     query: query,
                     cancellationToken: HttpContext.RequestAborted)
                 .ConfigureAwait(false);

        return new FundStatusResultViewModel(
            Retry: result.Retry,
            IsOrderExist: result.IsOrderExist);
    }

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
    ///             "vendorMemberId": "4ucmqplxb029",
    ///             "isMobile": false
    ///         }
    /// </remarks>
    [HttpPost("game/url", Name = "GameUrl")]
    public async ValueTask<GameUrlResultViewModel> GameUrlAsync(
        [FromServices] IGetService<GameUrlQuery, GameUrlResult> service,
        GameUrlViewModel source)
    {
        var query = new GameUrlQuery(
            source.ApiUrl,
            source.OperatorId,
            source.VendorId,
            source.VendorMemberId,
            source.IsMobile);

        var result = await service
                 .GetAsync(
                     query: query,
                     cancellationToken: HttpContext.RequestAborted)
                 .ConfigureAwait(false);

        return new GameUrlResultViewModel(
            result.IsPlayerExist,
            result.GameUrl);
    }

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
    public async ValueTask<GameHistoryResultViewModel> GameHistoryAsync(
        [FromServices] IGetService<GameHistoryQuery, GameHistoryResult> service,
        GameHistoryViewModel source)
    {
        var query = new GameHistoryQuery(
            ApiUrl: source.ApiUrl,
            OperatorId: source.OperatorId,
            VendorId: source.VendorId,
            VersionKey: source.VersionKey);

        var result = await service
                .GetAsync(
                    query: query,
                    cancellationToken: HttpContext.RequestAborted)
                .ConfigureAwait(false);

        if (!result.Details.Any())
        {
            return new GameHistoryResultViewModel(
                LastVersionKey: result.LastVersionKey,
                Details: Enumerable.Empty<GameHistoryDetailViewModel>());
        }

        var details = result.Details.Select(detail => new GameHistoryDetailViewModel(
            PlayerCode: detail.PlayerCode,
            ExternalRoundId: detail.ExternalRoundId,
            IsCashout: detail.IsCashout,
            IsClosed: detail.IsClosed,
            Status: detail.Status.StringConvertToEnum(),
            ExternalCreatedOn: detail.ExternalCreatedOn,
            ExternalClosedOn: detail.ExternalClosedOn,
            Bet: detail.Bet,
            ValidBet: detail.ValidBet,
            PlayerWinLoss: detail.PlayerWinLoss,
            Turnover: detail.Turnover,
            Settle: detail.Settle,
            AdditionalData: detail.AdditionalData));

        return new GameHistoryResultViewModel(
            LastVersionKey: result.LastVersionKey,
            Details: details);
    }

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
    public async IAsyncEnumerable<GameHistoryDetailViewModel> GameHistorySettledByTimeframeAsync(
        [FromServices] IQueryService<GameHistorySettledByTimeframeQuery, GameHistoryDetail> service,
        GameHistoryByTimeframeViewModel source)
    {
        var query = new GameHistorySettledByTimeframeQuery(
            ApiUrl: source.ApiUrl,
            OperatorId: source.OperatorId,
            VendorId: source.VendorId,
            StartTime: source.StartTime,
            EndTime: source.EndTime);

        var details = await service.QueryAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ToArrayAsync(
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameHistoryDetail>();

        if (!details.Any())
        {
            yield break;
        }

        foreach (var detail in details)
        {
            yield return new GameHistoryDetailViewModel(
                PlayerCode: detail.PlayerCode,
                ExternalRoundId: detail.ExternalRoundId,
                IsCashout: detail.IsCashout,
                IsClosed: detail.IsClosed,
                Status: detail.Status.StringConvertToEnum(),
                ExternalCreatedOn: detail.ExternalCreatedOn,
                ExternalClosedOn: detail.ExternalClosedOn,
                Bet: detail.Bet,
                ValidBet: detail.ValidBet,
                PlayerWinLoss: detail.PlayerWinLoss,
                Turnover: detail.Turnover,
                Settle: detail.Settle,
                AdditionalData: detail.AdditionalData);
        }
    }

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
    public async IAsyncEnumerable<GameHistoryDetailViewModel> GameHistoryUnsettledByTimeframeAsync(
        [FromServices] IQueryService<GameHistoryUnsettledByTimeframeQuery, GameHistoryDetail> service,
        GameHistoryByTimeframeViewModel source)
    {
        var query = new GameHistoryUnsettledByTimeframeQuery(
            ApiUrl: source.ApiUrl,
            OperatorId: source.OperatorId,
            VendorId: source.VendorId,
            StartTime: source.StartTime,
            EndTime: source.EndTime);

        var details = await service.QueryAsync(
            query: query,
            cancellationToken: HttpContext.RequestAborted)
            .ToArrayAsync(
            cancellationToken: HttpContext.RequestAborted)
            .ConfigureAwait(false)
            ?? Enumerable.Empty<GameHistoryDetail>();

        if (!details.Any())
        {
            yield break;
        }

        foreach (var detail in details)
        {
            yield return new GameHistoryDetailViewModel(
                PlayerCode: detail.PlayerCode,
                ExternalRoundId: detail.ExternalRoundId,
                IsCashout: detail.IsCashout,
                IsClosed: detail.IsClosed,
                Status: detail.Status.StringConvertToEnum(),
                ExternalCreatedOn: detail.ExternalCreatedOn,
                ExternalClosedOn: detail.ExternalClosedOn,
                Bet: detail.Bet,
                ValidBet: detail.ValidBet,
                PlayerWinLoss: detail.PlayerWinLoss,
                Turnover: detail.Turnover,
                Settle: detail.Settle,
                AdditionalData: detail.AdditionalData);
        }
    }

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
    ///             "vendorMemberId": "4ucmqplxb029",
    ///             "oddsType": 2,
    ///             "currency": 20
    ///         }
    /// </remarks>
    [HttpPost("player/create", Name = "PlayerCreate")]
    public ValueTask PlayerCreateAsync(
        [FromServices] ICommandService<PlayerCreateCommand> service,
        PlayerCreateViewModel source)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: source.ApiUrl,
            OperatorId: source.OperatorId,
            VendorId: source.VendorId,
            BrandCode: source.BrandCode,
            VendorMemberId: source.VendorMemberId,
            OddsType: source.OddsType,
            Currency: source.Currency);
        return service
           .ExecuteAsync(
                command: command,
                cancellationToken: HttpContext.RequestAborted);
    }

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
    ///             "vendorMemberId": "4ucmqplxb029"
    ///         }
    /// </remarks>
    [HttpPost("player/balance", Name = "PlayerBalance")]
    public async ValueTask<PlayerBalanceResultViewModel> PlayerBalanceAsync(
        [FromServices] IGetService<PlayerBalanceQuery, PlayerBalanceResult> service,
        PlayerBalanceViewModel source)
    {
        var query = new PlayerBalanceQuery(
            source.ApiUrl,
            source.OperatorId,
            source.VendorId,
            source.VendorMemberId);

        var result = await service
                 .GetAsync(
                     query: query,
                     cancellationToken: HttpContext.RequestAborted)
                 .ConfigureAwait(false);

        return new PlayerBalanceResultViewModel(
            IsPlayerExist: result.IsPlayerExist,
            Balance: result.Balance,
            FreeBalance: result.FreeBalance,
            OutstandingBalance: result.OutstandingBalance);
    }

    /// <summary>
    /// Players the kick asynchronous.
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
    ///             "vendorMemberId": "4ucmqplxb029"
    ///         }
    /// </remarks>
    [HttpPost("player/kick", Name = "PlayerKick")]
    public ValueTask PlayerKickAsync(
        [FromServices] ICommandService<PlayerKickCommand> service,
        PlayerKickViewModel source)
    {
        var command = new PlayerKickCommand(
            source.ApiUrl,
            source.OperatorId,
            source.VendorId,
            source.VendorMemberId);

        return service
          .ExecuteAsync(
            command: command,
            cancellationToken: HttpContext.RequestAborted);
    }
}
