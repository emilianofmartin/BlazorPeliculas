using BlazorWebAssemblyIdiomas.Client;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization();

var host = builder.Build(); //Necesitamos una ref de IJSRuntime
var js = host.Services.GetRequiredService<IJSRuntime>();
var culture = await js.InvokeAsync<string>("culture.get");
var thisCulture = new CultureInfo("en-US");

if(culture != null)
    thisCulture = new CultureInfo(culture);

CultureInfo.DefaultThreadCurrentCulture = thisCulture;
CultureInfo.DefaultThreadCurrentUICulture = thisCulture;

await builder.Build().RunAsync();
