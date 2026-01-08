using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SaveMySpot;
using SaveMySpot.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseAddress = builder.Configuration["Api:BaseAddress"]
                     ?? throw new InvalidOperationException("Api:BaseAddress is not configured.");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });
builder.Services.AddScoped<FoodOrdersClient>();
builder.Services.AddScoped<ToastService>();

await builder.Build().RunAsync();
