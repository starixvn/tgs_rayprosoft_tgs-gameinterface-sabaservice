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

	public async ValueTask<FundReply> FundInAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, string orderId, int currency, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			PlayerCode = playerCode,
			OrderId = orderId,
			Currency = currency,
			Amount = amount
		};

		return await client.FundInAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<FundReply> FundOutAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, string orderId, int currency, decimal amount, CancellationToken cancellationToken = default)
	{
		var request = new FundRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			PlayerCode = playerCode,
			OrderId = orderId,
			Currency = currency,
			Amount = amount
		};

		return await client.FundOutAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<FundStatusReply> FundStatusAsync(Uri apiUrl, string operatorId, string vendorId, string orderId, CancellationToken cancellationToken = default)
	{
		var request = new FundStatusRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			OrderId = orderId
		};

		return await client.FundStatusAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<GameHistoryReply> GameHistoryAsync(Uri apiUrl, string operatorId, string vendorId, long versionKey, CancellationToken cancellationToken = default)
	{
		var request = new GameHistoryRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			VersionKey = versionKey
		};

		return await client.GameHistoryAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async IAsyncEnumerable<GameHistory> GameHistorySettledAsync(Uri apiUrl, string operatorId, string vendorId, DateTime startTime, DateTime endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameHistoryByTimeframeRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			StartTime = startTime.ToTimestamp(),
			EndTime = endTime.ToTimestamp()
		};

		var reply = await client.GameHistorySettledAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var items = reply.Items ?? Enumerable.Empty<GameHistory>();

		if (!items.Any())
		{
			yield break;
		}

		foreach (var item in items)
		{
			yield return item;
		}
	}

	public async IAsyncEnumerable<GameHistory> GameHistoryUnsettledAsync(Uri apiUrl, string operatorId, string vendorId, DateTime startTime, DateTime endTime, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var request = new GameHistoryByTimeframeRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			StartTime = startTime.ToTimestamp(),
			EndTime = endTime.ToTimestamp()
		};

		var reply = await client.GameHistoryUnsettledAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var items = reply.Items ?? Enumerable.Empty<GameHistory>();

		if (!items.Any())
		{
			yield break;
		}

		foreach (var item in items)
		{
			yield return item;
		}
	}

	public async ValueTask<GameUrlReply> GameUrlAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, bool isMobile, CancellationToken cancellationToken = default)
	{
		var request = new GameUrlRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			PlayerCode = playerCode,
			IsMobile = isMobile,
		};

		return await client.GameUrlAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerBalanceRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			PlayerCode = playerCode
		};

		return await client.PlayerBalanceAsync(
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

	public async ValueTask PlayerLogoutAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerOnlineRequest()
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			PlayerCode = playerCode
		};

		_ = await client.PlayerOnlineAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}

	public async ValueTask<bool> PlayerOnlineAsync(Uri apiUrl, string operatorId, string vendorId, string playerCode, CancellationToken cancellationToken = default)
	{
		var request = new PlayerOnlineRequest
		{
			ApiUrl = apiUrl.ToString(),
			OperatorId = operatorId,
			VendorId = vendorId,
			PlayerCode = playerCode
		};

		var reply = await client.PlayerOnlineAsync(
			request: request,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return reply.Online;
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
