using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Webgame.Blazor;
using Webgame.Blazor.Api;
using Webgame.Blazor.State;

namespace Webgame.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.RootComponents.Add<HeadOutlet>("head::after");

            var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
                ?? throw new InvalidOperationException("Missing configuration value: ApiBaseUrl");

            builder.Services.AddScoped<PlayerSession>();
            builder.Services.AddScoped<AuthMessageHandler>();
            builder.Services.AddScoped<PlayerUiState>();

            builder.Services.AddScoped(sp =>
            {
                var handler = sp.GetRequiredService<AuthMessageHandler>();
                handler.InnerHandler = new HttpClientHandler();

                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(apiBaseUrl)
                };
            });

            builder.Services.AddScoped<ApiClient>();

            await builder.Build().RunAsync();
        }
    }
}