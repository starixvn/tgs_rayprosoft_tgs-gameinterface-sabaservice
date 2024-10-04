using System.Net;
using System.Reflection;
using Tgs.GameInterface.SabaService.Core;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddGameInterfaceCore(
		this IServiceCollection services, Settings settings)
		=> services
		.RegisterAllServiceUtilityTypes(
			Assembly.GetExecutingAssembly(),
			ServiceLifetime.Transient)
		.AddSingleton(settings)
		.AddHttpClient<IGameApiClient, GameApiClient>()
		.ConfigurePrimaryHttpMessageHandler(() =>
		{
			if (settings.Proxy != null)
			{
				var proxy = new WebProxy(settings.Proxy);

				return new HttpClientHandler
				{
					Proxy = proxy,
				};
			}

			return new HttpClientHandler();
		})
		.Services
		.AddSingleton<IGameVendorMember, GameVendorMember>()
		.AddSingleton<IGameHistoryCalculate, GameHistoryCalculate>();
}
