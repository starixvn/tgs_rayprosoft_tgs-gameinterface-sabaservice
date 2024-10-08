using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Tgs.GameInterface.SabaService.Core.ApplicationServices;
using Tgs.GameInterface.SabaService.Protos;
using Tgs.ServiceUtility;

namespace Tgs.GameInterface.SabaService.GrpcServices;

public class GameInterfaceSabaGrpcService(
	ILogger<GameInterfaceSabaGrpcService> logger,
	IGetService<GameUrlQuery, GameUrlResult> gameUrlService,
	IGetService<StatementReportTokenQuery, StatementTokenResult> stateTokenService,
	ICommandService<PlayerCreateCommand> playerCreateService) : GameInterfaceSabaService.GameInterfaceSabaServiceBase
{
	public override Task<CheckingServiceReply> CheckingService(CheckingServiceRequest request, ServerCallContext context)
	{
		logger.LogWarning("LogOn:{logOn} Message: Create a check connection.({machineName})", DateTime.Now.ToString("o"), request.MachineName);

		return Task.FromResult(new CheckingServiceReply
		{
			MachineName = Environment.MachineName
		});
	}

	public override async Task<StatementReportTokenReply> StatementReportToken(StatementReportTokenRequest request, ServerCallContext context)
	{
		var query = new StatementReportTokenQuery(
			ApiUrl: new Uri(request.ApiUrl),
			Username: request.Username,
			Password: request.Password);

		var result = await stateTokenService.GetAsync(
			query: query,
			cancellationToken: context.CancellationToken)
			.ConfigureAwait(false);

		return new StatementReportTokenReply
		{
			AccessToken = result.AccessToken,
			ExpriesIn = result.ExpiresIn,
			TokenType = result.TokenType
		};
	}

	public override async Task<GameUrlReply> GameUrl(GameUrlRequest request, ServerCallContext context)
	{
		var query = new GameUrlQuery(
			ApiUrl: new Uri(request.ApiUrl),
			OperatorId: request.OperatorId,
			VendorId: request.VendorId,
			PlayerCode: request.PlayerCode,
			IsMobile: request.IsMobile,
			BrandCode: request.BrandCode);

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
}
