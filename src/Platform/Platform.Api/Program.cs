using Platform.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<PlatformSettings>(
    builder.Configuration.GetSection(nameof(PlatformSettings)));

var platformSettings = new PlatformSettings();
builder.Configuration.Bind(nameof(PlatformSettings), platformSettings);
platformSettings.Validate();

builder.Services.AddPlatform(platformSettings);
builder.Services.AddWebApi();

var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}

var app = builder.Build();

app.UseWebApi(app.Environment);

app.Run();
