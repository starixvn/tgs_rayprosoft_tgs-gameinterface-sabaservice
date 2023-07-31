using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.GrpcServices;

public class GameInterfaceSabaGrpcService : GameInterfaceSabaService.GameInterfaceSabaServiceBase
{
    private readonly ILogger<GameInterfaceSabaGrpcService> _logger;
    private readonly IGetService<FundInQuery, FundResult> _fundInService;
    private readonly IGetService<FundOutQuery, FundResult> _fundOutService;
    private readonly IGetService<FundStatusQuery, FundStatusResult> _fundStatusService;
    private readonly IGetService<GameHistoryQuery, GameHistoryResult> _gameHistoryService;
    private readonly IQueryService<GameHistorySettledByTimeframeQuery, GameHistoryDetail> _gameHistorySettledByTimeframeService;
    private readonly IQueryService<GameHistoryUnsettledByTimeframeQuery, GameHistoryDetail> _gameHistoryUnsettledByTimeframeService;
    private readonly IGetService<GameUrlQuery, GameUrlResult> _gameUrlService;
    private readonly IGetService<PlayerBalanceQuery, PlayerBalanceResult> _playerBalanceService;
    private readonly ICommandService<PlayerCreateCommand> _playerCreateService;
    private readonly ICommandService<PlayerKickCommand> _playerKickService;

    public GameInterfaceSabaGrpcService(
        ILogger<GameInterfaceSabaGrpcService> logger,
        IGetService<FundInQuery, FundResult> fundInService,
        IGetService<FundOutQuery, FundResult> fundOutService,
        IGetService<FundStatusQuery, FundStatusResult> fundStatusService,
        IGetService<GameHistoryQuery, GameHistoryResult> gameHistoryService,
        IQueryService<GameHistorySettledByTimeframeQuery, GameHistoryDetail> gameHistorySettledByTimeframeService,
        IQueryService<GameHistoryUnsettledByTimeframeQuery, GameHistoryDetail> gameHistoryUnsettledByTimeframeService,
        IGetService<GameUrlQuery, GameUrlResult> gameUrlService,
        IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
        ICommandService<PlayerCreateCommand> playerCreateService,
        ICommandService<PlayerKickCommand> playerKickService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _fundInService = fundInService ?? throw new ArgumentNullException(nameof(fundInService));
        _fundOutService = fundOutService ?? throw new ArgumentNullException(nameof(fundOutService));
        _fundStatusService = fundStatusService ?? throw new ArgumentNullException(nameof(fundStatusService));
        _gameHistoryService = gameHistoryService ?? throw new ArgumentNullException(nameof(gameHistoryService));
        _gameHistorySettledByTimeframeService = gameHistorySettledByTimeframeService ?? throw new ArgumentNullException(nameof(gameHistorySettledByTimeframeService));
        _gameHistoryUnsettledByTimeframeService = gameHistoryUnsettledByTimeframeService ?? throw new ArgumentNullException(nameof(gameHistoryUnsettledByTimeframeService));
        _gameUrlService = gameUrlService ?? throw new ArgumentNullException(nameof(gameUrlService));
        _playerBalanceService = playerBalanceService ?? throw new ArgumentNullException(nameof(playerBalanceService));
        _playerCreateService = playerCreateService ?? throw new ArgumentNullException(nameof(playerCreateService));
        _playerKickService = playerKickService ?? throw new ArgumentNullException(nameof(playerKickService));
    }

    public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
    {
        _logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

        return Task.FromResult(new CheckingServiceReply
        {
            MachineName = Environment.MachineName
        });
    }

    public override async Task<FundReply> FundIn(FundRequest request, ServerCallContext context)
    {
        var query = new FundInQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            VendorMemberId: request.VendorMemberId,
            VendorTransId: request.VendorTransId,
            Currency: request.Currency,
            Amount: request.Amount);

        var result = await _fundInService
                    .GetAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        return new FundReply
        {
            Check = result.Check,
            IsOrderExist = result.IsOrderExist,
            VendorTransId = result.VendorTransId,
            ActualVendorTransId = result.ActualVendorTransId,
            TransId = result.TransId,
            Balance = result.Balance
        };
    }

    public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
    {
        var query = new FundOutQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            VendorMemberId: request.VendorMemberId,
            VendorTransId: request.VendorTransId,
            Currency: request.Currency,
            Amount: request.Amount);

        var result = await _fundOutService
                    .GetAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        return new FundReply
        {
            Check = result.Check,
            IsOrderExist = result.IsOrderExist,
            VendorTransId = result.VendorTransId,
            ActualVendorTransId = result.ActualVendorTransId,
            TransId = result.TransId,
            Balance = result.Balance
        };
    }

    public override async Task<FundStatusReply> FundStatus(FundStatusRequest request, ServerCallContext context)
    {
        var query = new FundStatusQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            VendorTransId: request.VendorTransId);

        var result = await _fundStatusService
                    .GetAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        return new FundStatusReply
        {
            Retry = result.Retry,
            IsOrderExist = result.IsOrderExist,
        };
    }

    public override async Task<GameHistoryReply> GameHistory(GameHistoryRequest request, ServerCallContext context)
    {
        var query = new GameHistoryQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            VersionKey: request.VersionKey);

        var result = await _gameHistoryService
                    .GetAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        var reply = new GameHistoryReply
        {
            LastVersionKey = result.LastVersionKey
        };

        if (result.Details.Any())
        {
            reply.Items.AddRange(
                result.Details.Select(detail => new GameHistory()
                {
                    PlayerCode = detail.PlayerCode,
                    ExternalRoundId = detail.ExternalRoundId,
                    IsCashout = detail.IsCashout,
                    IsClosed = detail.IsClosed,
                    Status = detail.Status.StringConvertToEnum(),
                    ExternalCreatedOn = detail.ExternalCreatedOn.ToTimestamp(),
                    ExternalClosedOn = detail.ExternalClosedOn.ToTimestamp(),
                    Bet = detail.Bet,
                    ValidBet = detail.ValidBet,
                    PlayerWinLoss = detail.PlayerWinLoss,
                    Turnover = detail.Turnover,
                    Settle = detail.Settle,
                    AdditionalData = detail.AdditionalData
                })
            );
        }
        return reply;
    }

    public override async Task<GameHistoryByTimeframeReply> GameHistorySettled(GameHistoryByTimeframeRequest request, ServerCallContext context)
    {
        var query = new GameHistorySettledByTimeframeQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            StartTime: request.StartTime.ToDateTime(),
            EndTime: request.EndTime.ToDateTime());

        var details = await _gameHistorySettledByTimeframeService
                    .QueryAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ToArrayAsync(cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        var reply = new GameHistoryByTimeframeReply();

        if (details.Any())
        {
            foreach (var detail in details)
            {
                reply.Items.Add(
                    new GameHistory()
                    {
                        PlayerCode = detail.PlayerCode,
                        ExternalRoundId = detail.ExternalRoundId,
                        IsCashout = detail.IsCashout,
                        IsClosed = detail.IsClosed,
                        Status = detail.Status.StringConvertToEnum(),
                        ExternalCreatedOn = detail.ExternalCreatedOn.ToTimestamp(),
                        ExternalClosedOn = detail.ExternalClosedOn.ToTimestamp(),
                        Bet = detail.Bet,
                        ValidBet = detail.ValidBet,
                        PlayerWinLoss = detail.PlayerWinLoss,
                        Turnover = detail.Turnover,
                        Settle = detail.Settle,
                        AdditionalData = detail.AdditionalData
                    });
            }
        }
        return reply;
    }

    public override async Task<GameHistoryByTimeframeReply> GameHistoryUnsettled(GameHistoryByTimeframeRequest request, ServerCallContext context)
    {
        var query = new GameHistoryUnsettledByTimeframeQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            StartTime: request.StartTime.ToDateTime(),
            EndTime: request.EndTime.ToDateTime());

        var details = await _gameHistoryUnsettledByTimeframeService
                    .QueryAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ToArrayAsync(cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        var reply = new GameHistoryByTimeframeReply();

        if (details.Any())
        {
            foreach (var detail in details)
            {
                reply.Items.Add(
                    new GameHistory()
                    {
                        PlayerCode = detail.PlayerCode,
                        ExternalRoundId = detail.ExternalRoundId,
                        IsCashout = detail.IsCashout,
                        IsClosed = detail.IsClosed,
                        Status = detail.Status.StringConvertToEnum(),
                        ExternalCreatedOn = detail.ExternalCreatedOn.ToTimestamp(),
                        ExternalClosedOn = detail.ExternalClosedOn.ToTimestamp(),
                        Bet = detail.Bet,
                        ValidBet = detail.ValidBet,
                        PlayerWinLoss = detail.PlayerWinLoss,
                        Turnover = detail.Turnover,
                        Settle = detail.Settle,
                        AdditionalData = detail.AdditionalData
                    });
            }
        }
        return reply;
    }

    public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
    {
        var query = new GameUrlQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            VendorMemberId: request.VendorMemberId,
            IsMobile: request.IsMobile);

        var result = await _gameUrlService
                    .GetAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        return new GameUrlReply
        {
            IsPlayerExist = result.IsPlayerExist,
            GameUrl = result.GameUrl
        };
    }

    public override async Task<PlayerBalanceReply> PlayerBalance(PlayerBalanceRequest request, ServerCallContext context)
    {
        var query = new PlayerBalanceQuery(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            VendorMemberId: request.VendorMemberId);

        var result = await _playerBalanceService
                    .GetAsync(
                        query: query,
                        cancellationToken: context.CancellationToken)
                    .ConfigureAwait(false);

        return new PlayerBalanceReply
        {
            IsPlayerExist = result.IsPlayerExist,
            Balance = result.Balance,
            FreeBalance = result.FreeBalance,
            OutstandingBalance = result.OutstandingBalance
        };
    }

    public override async Task<BaseReply> PlayerCreate(PlayerCreateRequest request, ServerCallContext context)
    {
        var command = new PlayerCreateCommand(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            BrandCode: request.BrandCode,
            VendorMemberId: request.VendorMemberId,
            OddsType: request.OddsType,
            Currency: request.Currency);

        await _playerCreateService
            .ExecuteAsync(
                command: command,
                cancellationToken: context.CancellationToken)
            .ConfigureAwait(false);

        return new BaseReply
        {
            Status = true
        };
    }

    public override async Task<BaseReply> PlayerKick(PlayerKickRequest request, ServerCallContext context)
    {
        var command = new PlayerKickCommand(
            ApiUrl: new Uri(request.ApiUrl),
            OperatorId: request.OperatorId,
            VendorId: request.VendorId,
            VendorMemberId: request.VendorMemberId);

        await _playerKickService
                .ExecuteAsync(
                    command: command,
                    cancellationToken: context.CancellationToken)
                .ConfigureAwait(false);

        return new BaseReply
        {
            Status = true
        };
    }
}
