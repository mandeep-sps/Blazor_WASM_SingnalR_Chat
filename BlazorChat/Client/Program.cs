
using BlazorChat.Client;
using BlazorChat.Client.Managers;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddHttpClient("BlazorChat.ServerAPI", client => client.BaseAddress = new 
Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

// Supply HttpClient instances that include access tokens when making requests to the server project
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorChat.ServerAPI"));
builder.Services.AddMudServices(c => { c.SnackbarConfiguration.PositionClass =
    Defaults.Classes.Position.BottomRight; });
builder.Services.AddApiAuthorization();
builder.Services.AddTransient<IChatManager, ChatManager>();
await builder.Build().RunAsync();



