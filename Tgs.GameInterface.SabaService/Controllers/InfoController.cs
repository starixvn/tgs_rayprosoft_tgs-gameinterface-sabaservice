using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Core;

namespace Tgs.GameInterface.SabaService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class InfoController : ControllerBase
{
    private readonly GameInterfaceSettingsOptions _gameInterfaceSettings;

    public InfoController(IOptions<GameInterfaceSettingsOptions> options)
    {
        _gameInterfaceSettings = options.Value;
    }

    [HttpGet]
    public async ValueTask<object> Get([FromServices] IWebHostEnvironment environment)
    {
        var hostEntry = await Dns.GetHostEntryAsync(Dns.GetHostName());
        var hostIp = Array.Find(
                (await Dns
                    .GetHostEntryAsync(
                        Dns.GetHostName())
                    .ConfigureAwait(false))
                    .AddressList, x => x.AddressFamily == AddressFamily.InterNetwork);

        return new
        {
            Environment.MachineName,
            HostName = Dns.GetHostName(),
            HostIp = hostIp?.ToString() ?? string.Empty,
            environment.EnvironmentName,
            IsDevelopment = environment.IsDevelopment(),
            IsStaging = environment.IsStaging(),
            IsProduction = environment.IsProduction(),
            OsVersion = $"{Environment.OSVersion}",
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            Environment.ProcessorCount,
            _gameInterfaceSettings.ReleaseNamespace
        };
    }
}
