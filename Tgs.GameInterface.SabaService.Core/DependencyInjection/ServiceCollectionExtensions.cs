using System.Reflection;
using Tgs.GameInterface.SabaService.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameInterfaceCore(
        this IServiceCollection services)
        => services
        .RegisterAllServiceUtilityTypes(
            Assembly.GetExecutingAssembly(),
            ServiceLifetime.Transient)
        .AddHttpClient<IGameApiClient, GameApiClient>()
        .Services
        .AddSingleton<IGameVendorMember, GameVendorMember>()
        .AddSingleton<IGameHistoryCalculate, GameHistoryCalculate>();
}
