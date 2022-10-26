using Microsoft.Extensions.Options;
using Tgs.GameInterface.SabaService.Core;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddOptions<GameInterfaceSettingsOptions>()
    .Configure((options) =>
    {
        var releaseNamespace = builder.Configuration.GetValue<string>("RELEASE_NAMESPACE");
        options.ReleaseNamespace = releaseNamespace;
    });

builder.Services
    .AddHealthChecks();


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapHealthChecks("/healthz");

app.Run();
