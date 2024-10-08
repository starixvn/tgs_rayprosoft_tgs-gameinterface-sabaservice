using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameInterface.SabaService.Client;

internal class GameInterfaceSabaClient(GameInterfaceSabaService.GameInterfaceSabaServiceClient client) : IGameInterfaceSabaClient
{
	public async ValueTask<string> CheckingServiceAsync(string machineName, CancellationToken cancellationToken = default)
	{
		var reply = await client.CheckingServiceAsync(
				request: new CheckingServiceRequest
				{
					MachineName = machineName
				},
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.MachineName;
	}

	public async ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string vendorId, string playerCode, bool isMobile, string brandCode, CancellationToken cancellationToken = default)
	{
		var request = new GameUrlRequest()
		{
			ApiUrl = apiUrl.ToString(),
			VendorId = vendorId,
			PlayerCode = playerCode,
			IsMobile = isMobile,
			BrandCode = brandCode,
		};

		return await client.GameUrlAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask PlayerCreateAsync(Uri apiUrl, string operatorId, string vendorId, string brandCode, string playerCode, int oddsType, int currency, CancellationToken cancellationToken = default)
	{
		var request = new PlayerCreateRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			BrandCode = brandCode,
			PlayerCode = playerCode,
			OddsType = oddsType,
			Currency = currency
		};

		_ = await client.PlayerCreateAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<StatementReportTokenReply> StatementReportTokenAsync(Uri apiUrl, string username, string password, CancellationToken cancellationToken = default)
	{
		var request = new StatementReportTokenRequest()
		{
			ApiUrl = apiUrl.ToString(),
			Username = username,
			Password = password,
		};

		var reply = await client.StatementReportTokenAsync(
			request: request,
			cancellationToken: cancellationToken);

		return reply;
	}
}
