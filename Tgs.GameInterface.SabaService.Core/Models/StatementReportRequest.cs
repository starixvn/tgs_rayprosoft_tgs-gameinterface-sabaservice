using System.Net.Http.Json;

namespace Tgs.GameInterface.SabaService.Core.Models;

public record StatementReportRequest(Uri ApiUrl, string Path, JsonContent Content, CancellationToken CancellationToken);
