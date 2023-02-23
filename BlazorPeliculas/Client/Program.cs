using BlazorPeliculas.Client;
using BlazorPeliculas.Client.Auth;
using BlazorPeliculas.Client.Repositories;
using CurrieTechnologies.Razor.SweetAlert2;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
configureServices(builder.Services);
await builder.Build().RunAsync();

void configureServices(IServiceCollection services) {
    services.AddScoped<IRepository, Repository>(); /* Inyectar un IRepository. 
    Sin embargo, en tiempo de ejecución lo que se va a hacer es que se va a proveer una instancia de la clase Repository. */
    services.AddSweetAlert2();
    services.AddAuthorizationCore();

    services.AddScoped<JWTAuthProvider>();
    services.AddScoped<AuthenticationStateProvider, JWTAuthProvider>(provider =>
        provider.GetRequiredService<JWTAuthProvider>());

    services.AddScoped<ILoginService, JWTAuthProvider>(provider =>
        provider.GetRequiredService<JWTAuthProvider>());

    services.AddScoped<TokenRenewer>();
}
