using System.Diagnostics;

namespace Tgs.GameInterface.SabaService.Client;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GameInterface.SabaService.Client.DiagnosticsInterceptor");
}
