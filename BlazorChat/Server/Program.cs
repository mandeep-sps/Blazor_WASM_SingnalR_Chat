

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

builder.Services.AddResponseCompression(options =>
{
    System.Collections.Generic.IEnumerable<string> MimeTypes = new[]
     {
         // General
         "text/plain",
         "text/html",
         "text/css",
         "font/woff2",
         "application/javascript",
         "image/x-icon",
         "image/png",
         "application/octet-stream"
     };
    options.EnableForHttps = true;
    options.MimeTypes = MimeTypes;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});


var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

await app.RunAsync();