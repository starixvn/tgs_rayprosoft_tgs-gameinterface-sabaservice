using System.Runtime.CompilerServices;
using Google.Protobuf.WellKnownTypes;
using Tgs.GameInterface.SabaService.Protos;

namespace Tgs.GameInterface.SabaService.Client;

internal class GameInterfaceSabaClient(GameInterfaceSabaService.GameInterfaceSabaServiceClient client) : IGameInterfaceSabaClient
{
    public async ValueTask<string> CheckingServiceAsync(
        string machineName,
        CancellationToken cancellationToken = default)
    {
        var reply = await client
            .CheckingServiceAsync(
                request: new CheckingServiceRequest
                {
                    MachineName = machineName
                },
                cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        return reply.MachineName;
    }

    public async ValueTask<FundReply> FundInAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        string vendorTransId,
        int currency,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var request = new FundRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            VendorMemberId = vendorMemberId,
            VendorTransId = vendorTransId,
            Currency = currency,
            Amount = amount
        };

        return await client
                .FundInAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async ValueTask<FundReply> FundOutAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        string vendorTransId,
        int currency,
        decimal amount,
        CancellationToken cancellationToken = default)
    {
        var request = new FundRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            VendorMemberId = vendorMemberId,
            VendorTransId = vendorTransId,
            Currency = currency,
            Amount = amount
        };

        return await client
                .FundOutAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async ValueTask<FundStatusReply> FundStatusAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorTransId,
        CancellationToken cancellationToken = default)
    {
        var request = new FundStatusRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            VendorTransId = vendorTransId
        };

        return await client
                .FundStatusAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async ValueTask<GameHistoryReply> GameHistoryAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        long versionKey,
        CancellationToken cancellationToken = default)
    {
        var request = new GameHistoryRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            VersionKey = versionKey
        };

        return await client
                .GameHistoryAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async IAsyncEnumerable<GameHistory> GameHistorySettledAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        DateTime startTime,
        DateTime endTime,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
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

    public async IAsyncEnumerable<GameHistory> GameHistoryUnsettledAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        DateTime startTime,
        DateTime endTime,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
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

    public async ValueTask<GameUrlReply> GameUrlAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        bool isMobile,
        CancellationToken cancellationToken = default)
    {
        var request = new GameUrlRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            VendorMemberId = vendorMemberId,
            IsMobile = isMobile,
        };

        return await client
                .GameUrlAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async ValueTask<PlayerBalanceReply> PlayerBalanceAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        CancellationToken cancellationToken = default)
    {
        var request = new PlayerBalanceRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            VendorMemberId = vendorMemberId
        };

        return await client
                .PlayerBalanceAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async ValueTask PlayerCreateAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string brandCode,
        string vendorMemberId,
        int oddsType,
        int currency,
        CancellationToken cancellationToken = default)
    {
        var request = new PlayerCreateRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            BrandCode = brandCode,
            VendorMemberId = vendorMemberId,
            OddsType = oddsType,
            Currency = currency
        };

        _ = await client
                .PlayerCreateAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }

    public async ValueTask PlayerKickAsync(
        Uri apiUrl,
        string operatorId,
        string vendorId,
        string vendorMemberId,
        CancellationToken cancellationToken = default)
    {
        var request = new PlayerKickRequest()
        {
            ApiUrl = apiUrl.ToString(),
            OperatorId = operatorId,
            VendorId = vendorId,
            VendorMemberId = vendorMemberId
        };

        _ = await client
                .PlayerKickAsync(
                    request: request,
                    cancellationToken: cancellationToken)
                .ConfigureAwait(false);
    }
}
