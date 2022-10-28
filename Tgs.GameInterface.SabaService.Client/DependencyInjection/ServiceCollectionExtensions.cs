using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Client;
using Tgs.GameInterface.SabaService.Protos;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameInterfaceSabaServiceClient(
        this IServiceCollection services,
        Action<GameInterfaceSabaClientOptions, IServiceProvider> configure)
        => services
            .AddOptions<GameInterfaceSabaClientOptions>()
            .Configure(configure)
            .Services
            .AddGrpcClient<GameInterfaceSabaService.GameInterfaceSabaServiceClient>((sp, configureClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameInterfaceSabaClientOptions>>().Value;
                configureClient.Address = options.BaseUrl;
            })
            .ConfigureChannel((sp, configureChannel) =>
            {
                var handler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
                configureChannel.HttpHandler = handler;
            })
            .AddInterceptor<LoggingInterceptor>()
            .Services
            .AddSingleton<DiagnosticsInterceptorOptions>()
            .AddSingleton<LoggingInterceptor>()
            .AddTransient<IGameInterfaceSabaClient, GameInterfaceSabaClient>();
}
