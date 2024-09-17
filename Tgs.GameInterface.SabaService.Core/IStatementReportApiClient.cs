using System.Net.Http.Json;
using Tgs.GameInterface.SabaService.Core.Models;

namespace Tgs.GameInterface.SabaService.Core;

public interface IStatementReportApiClient
{
	Task<TResult> PostAsync<TResult>(StatementReportRequest request);
}
