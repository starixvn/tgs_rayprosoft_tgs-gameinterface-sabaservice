using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace Tgs.GameInterface.SabaService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public InfoController(IConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        public async ValueTask<object> Get([FromServices] IWebHostEnvironment environment)
        {
            var hostName = Dns.GetHostName();
            var hostEntry = await Dns.GetHostEntryAsync(hostName);
            var hostIp = Array.Find(hostEntry.AddressList,
                x => x.AddressFamily == AddressFamily.InterNetwork);

            var Namespace = _configuration.GetSection("GameInterface:Namespace").Value;
            return new
            {
                Data = new
                {
                    MachineName = Environment.MachineName,
                    HostName = hostName,
                    HostIp = hostIp?.ToString() ?? string.Empty,
                    EnvironmentName = environment.EnvironmentName,
                    IsDevelopment = environment.IsDevelopment(),
                    IsProduction = environment.IsProduction(),
                    OsVersion = $"{Environment.OSVersion}",
                    Version = Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                    ProcessCount = Environment.ProcessorCount,
                    Namespace = Namespace
                }
            };
        }
    }
}
