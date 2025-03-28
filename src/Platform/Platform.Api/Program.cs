using Platform.Api;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PlatformSettings>(
    builder.Configuration.GetSection(nameof(PlatformSettings)));

var platformSettings = new PlatformSettings();
builder.Configuration.Bind(nameof(PlatformSettings), platformSettings);
platformSettings.Validate();

builder.Services.AddPlatform(platformSettings);
builder.Services.AddWebApi(platformSettings.SecurityTokenSettings);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

app.UseWebApi(app.Environment);

app.Run();
