using Microsoft.EntityFrameworkCore;
using Webgame.Api.Common;
using Webgame.Application.Leaderboards;
using Webgame.Application.Persistence;
using Webgame.Application.Players;
using Webgame.Application.Upgrades;
using Webgame.Infrastructure.Leaderboards;
using Webgame.Infrastructure.Persistence;
using Webgame.Infrastructure.Players;
using Webgame.Infrastructure.Upgrades;

namespace Webgame.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            const string FrontendCorsPolicy = "FrontendCors";

            var connectionString = builder.Configuration.GetConnectionString("WebgameDb");
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Missing connection string 'ConnectionStrings:WebgameDb'. " +
                    "Set it in User Secrets locally or Azure App Service settings in production.");
            }

            var allowedOrigins = builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? Array.Empty<string>();

            Console.WriteLine("DB configured: " + (!string.IsNullOrWhiteSpace(connectionString)));
            Console.WriteLine("Allowed CORS origins: " + string.Join(", ", allowedOrigins));

            #region Services

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Application
            builder.Services.AddScoped<PlayerService>();

            // Infrastructure
            builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
            builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();
            builder.Services.AddScoped<ILeaderboardQuery, EfLeaderboardQuery>();
            builder.Services.AddScoped<IUpgradeCatalogQuery, EfUpgradeCatalogQuery>();

            // Persistence
            builder.Services.AddDbContext<WebgameDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Common
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(FrontendCorsPolicy, policy =>
                {
                    if (allowedOrigins.Length == 0)
                    {
                        return;
                    }

                    policy
                        .WithOrigins(allowedOrigins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            #endregion

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseCors(FrontendCorsPolicy);
            app.UseAuthorization();

            app.MapGet("/health", () => Results.Ok(new
            {
                status = "ok",
                environment = app.Environment.EnvironmentName
            }));

            app.MapControllers();

            app.Run();
        }
    }
}