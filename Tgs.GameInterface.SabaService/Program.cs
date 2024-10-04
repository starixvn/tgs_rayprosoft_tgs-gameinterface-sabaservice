using System.Net.Mime;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Tgs.GameInterface.SabaService;
using Tgs.GameInterface.SabaService.Core;
using Tgs.GameInterface.SabaService.Core.Models;
using Tgs.GameInterface.SabaService.GrpcServices;

var filterInstrumentations = new[] { "html", "ico", "png", "jpg" };

var builder = WebApplication.CreateBuilder(args);
var settings = builder.Configuration.Get<Settings>();

builder.WebHost.UseSentry();

builder.Logging
	.AddOpenTelemetry(options =>
	{
		options.IncludeFormattedMessage = true;
		options.ParseStateValues = true;
		_ = options.AddFluentdForwardExporter(fluentOptions =>
		{
			var nodeIp = builder.Configuration.GetValue<string>("Otel:NodeIp")!;
			fluentOptions.Host = nodeIp;
			fluentOptions.Tag = builder.Configuration.GetValue<string>("Otel:ServiceName")!;
			fluentOptions.UseMessagePack();
		});
	});

// Add services to the container.

builder.Services
	.AddRouting(options => options.LowercaseUrls = true)
	.AddControllers(options =>
	{
		options.Filters.Add(new ProducesAttribute(MediaTypeNames.Application.Json));
		options.Filters.Add(new ConsumesAttribute(MediaTypeNames.Application.Json));
	})
	.AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// Additional configuration is required to successfully run gRPC on macOS.
// For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

// Add services to the container.
builder.Services.AddSingleton<DiagnosticsInterceptorOptions>();
builder.Services.AddSingleton<LoggingInterceptor>();
builder.Services.AddGrpc(options =>
{
	options.Interceptors.Add<LoggingInterceptor>();
	// If true, detailed exception messages are returned to clients when an exception is thrown in a service method.
	// The default is false. Setting EnableDetailedErrors to true can leak sensitive information.
	options.EnableDetailedErrors = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	var xmlFileNames = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.xml");
	foreach (var xmlFileName in xmlFileNames)
	{
		c.IncludeXmlComments(xmlFileName);
	}
});

builder.Services
	.AddOptions<GameInterfaceSettingsOptions>()
	.Configure((options) =>
	{
		var releaseNamespace = builder.Configuration.GetValue<string>("RELEASE_NAMESPACE")!;
		options.ReleaseNamespace = releaseNamespace;
	})
	.Services
	.AddGameInterfaceCore(settings);

builder.Services
	.AddOpenTelemetry()
	.ConfigureResource(_ => _.AddService(builder.Configuration.GetValue<string>("Otel:ServiceName")))
	.WithTracing(tracerProviderBuilder => tracerProviderBuilder
		.AddSource("Tgs.*")
		.AddSource("TGS.*")
		.AddSource("Valhalla.*")
		.AddAspNetCoreInstrumentation(options => options.Filter =
			httpContext => !httpContext.Request.Path.HasValue
			|| !httpContext.Request.Path.StartsWithSegments("/swagger")
			&& !httpContext.Request.Path.StartsWithSegments("/health")
			&& !httpContext.Request.Path.StartsWithSegments("/profiler/")
			&& !httpContext.Request.Path.StartsWithSegments("/hangfire")
			&& !httpContext.Request.Path.StartsWithSegments("/metrics")
			&& !filterInstrumentations.Any(ext => httpContext.Request.Path.Value.EndsWith($".{ext}")))
		.AddHttpClientInstrumentation()
		.AddGrpcClientInstrumentation()
		.AddGrpcCoreInstrumentation()
		.AddEntityFrameworkCoreInstrumentation(options => options.SetDbStatementForText = true)
		.AddOtlpExporter(options => options.Endpoint = builder.Configuration.GetValue<Uri>("Otel:Collector")))
	.WithMetrics(meterBuiler => meterBuiler
	.SetResourceBuilder(ResourceBuilder.CreateDefault()
		.AddEnvironmentVariableDetector())
		.AddRuntimeInstrumentation()
		.AddAspNetCoreInstrumentation()
		.AddOtlpExporter())
	.Services
	.AddInterceptionService()
	.AddServiceUtiltiyInterceptors();

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
_ = app.UseSwagger();
_ = app.UseSwaggerUI();

_ = app.UseHttpsRedirection();

_ = app.UseAuthorization();

_ = app.MapControllers();

app.MapHealthChecks("/health");

// Configure the HTTP request pipeline.
app.MapGrpcService<GameInterfaceSabaGrpcService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
