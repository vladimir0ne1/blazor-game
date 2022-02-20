using Blazored.Toast;
using BlazorGame.Client;
using BlazorGame.Client.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<IBananaService, BananaService>();
builder.Services.AddScoped<IUnitService, UnitService>();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
