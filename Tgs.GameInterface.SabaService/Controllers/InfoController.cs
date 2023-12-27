using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Core;

namespace Tgs.GameInterface.SabaService.Controllers;

[Route("[controller]")]
[ApiController]
public class InfoController(
    IOptions<GameInterfaceSettingsOptions> options) : ControllerBase
{
    private readonly GameInterfaceSettingsOptions _gameInterfaceSettings = options.Value;

    [HttpGet]
    public async ValueTask<object> GetAsync(
        [FromServices] IWebHostEnvironment _hostingEnvironment)
    {
        var hostName = Dns.GetHostName();
        var hostEntry = await Dns.GetHostEntryAsync(hostName);
        var hostIp = Array.Find(hostEntry.AddressList,
            x => x.AddressFamily == AddressFamily.InterNetwork);

        return new
        {
            Environment.MachineName,
            HostName = hostName,
            HostIp = hostIp?.ToString() ?? string.Empty,
            Environment = _hostingEnvironment.EnvironmentName,
            OsVersion = $"{Environment.OSVersion}",
            Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
            IsDevelopment = _hostingEnvironment.IsDevelopment(),
            IsStaging = _hostingEnvironment.IsStaging(),
            IsProduction = _hostingEnvironment.IsProduction(),
            ProcessCount = Environment.ProcessorCount,
            _gameInterfaceSettings.ReleaseNamespace,
        };
    }
}
