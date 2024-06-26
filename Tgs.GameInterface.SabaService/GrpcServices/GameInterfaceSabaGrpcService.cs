using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.GrpcServices;

public class GameInterfaceSabaGrpcService(
	ILogger<GameInterfaceSabaGrpcService> logger,
	IGetService<FundInQuery, FundResult> fundInService,
	IGetService<FundOutQuery, FundResult> fundOutService,
	IGetService<FundStatusQuery, FundStatusResult> fundStatusService,
	IGetService<GameHistoryQuery, GameHistoryResult> gameHistoryService,
	IQueryService<GameHistorySettledByTimeframeQuery, GameHistoryDetail> gameHistorySettledByTimeframeService,
	IQueryService<GameHistoryUnsettledByTimeframeQuery, GameHistoryDetail> gameHistoryUnsettledByTimeframeService,
	IGetService<GameUrlQuery, GameUrlResult> gameUrlService,
	IGetService<PlayerBalanceQuery, PlayerBalanceResult> playerBalanceService,
	IGetService<PlayerOnlineQuery, bool> playerOnlineService,
	ICommandService<PlayerCreateCommand> playerCreateService,
	ICommandService<PlayerLogoutCommand> playerLogoutService) : GameInterfaceSabaService.GameInterfaceSabaServiceBase
{
	public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
	{
		logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

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
			PlayerCode: request.PlayerCode,
			OrderId: request.OrderId,
			Currency: request.Currency,
			Amount: request.Amount);

		var result = await fundInService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			Check = result.Check,
			IsOrderExist = result.IsOrderExist,
			OrderId = result.OrderId,
			VendorTransId = result.VendorTransId,
			ActualVendorTransId = result.ActualVendorTransId,
			TransId = result.TransId,
			Balance = result.Balance,
			Resp = result.Resp
		};
	}

	public override async Task<FundReply> FundOut(FundRequest request, ServerCallContext context)
	{
		var query = new FundOutQuery(
			ApiUrl: new Uri(request.ApiUrl),
			OperatorId: request.OperatorId,
			VendorId: request.VendorId,
			PlayerCode: request.PlayerCode,
			OrderId: request.OrderId,
			Currency: request.Currency,
			Amount: request.Amount);

		var result = await fundOutService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new FundReply
		{
			Check = result.Check,
			IsOrderExist = result.IsOrderExist,
			OrderId = result.OrderId,
			VendorTransId = result.VendorTransId,
			ActualVendorTransId = result.ActualVendorTransId,
			TransId = result.TransId,
			Balance = result.Balance,
			Resp = result.Resp
		};
	}

	public override async Task<FundStatusReply> FundStatus(FundStatusRequest request, ServerCallContext context)
	{
		var query = new FundStatusQuery(
			ApiUrl: new Uri(request.ApiUrl),
			OperatorId: request.OperatorId,
			VendorId: request.VendorId,
			OrderId: request.OrderId);

		var result = await fundStatusService.GetAsync(
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

		var result = await gameHistoryService.GetAsync(
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

		var details = await gameHistorySettledByTimeframeService.QueryAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ToArrayAsync(cancellationToken: context.CancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameHistoryDetail>();

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

		var details = await gameHistoryUnsettledByTimeframeService.QueryAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ToArrayAsync(cancellationToken: context.CancellationToken)
			.ConfigureAwait(false)
			?? Enumerable.Empty<GameHistoryDetail>();

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
			PlayerCode: request.PlayerCode,
			IsMobile: request.IsMobile);

		var result = await gameUrlService.GetAsync(
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
			PlayerCode: request.PlayerCode);

		var result = await playerBalanceService.GetAsync(
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
			PlayerCode: request.PlayerCode,
			OddsType: request.OddsType,
			Currency: request.Currency);

		await playerCreateService.ExecuteAsync(
			command: command,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new BaseReply
		{
			Status = true
		};
	}

	public override async Task<BaseReply> PlayerLogout(PlayerLogoutRequest request, ServerCallContext context)
	{
		var command = new PlayerLogoutCommand(
			ApiUrl: new Uri(request.ApiUrl),
			OperatorId: request.OperatorId,
			VendorId: request.VendorId,
			PlayerCode: request.PlayerCode);

		await playerLogoutService.ExecuteAsync(
			command: command,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new BaseReply
		{
			Status = true
		};
	}

	public override async Task<PlayerOnlineReply> PlayerOnline(PlayerOnlineRequest request, ServerCallContext context)
	{
		var query = new PlayerOnlineQuery(
			ApiUrl: new Uri(request.ApiUrl),
			OperatorId: request.OperatorId,
			VendorId: request.VendorId,
			PlayerCode: request.PlayerCode);

		var result = await playerOnlineService.GetAsync(
			   query: query,
			   cancellationToken: context.CancellationToken)
			   .ConfigureAwait(false);

		return new PlayerOnlineReply
		{
			Online = result
		};
	}
}
