
using Webgame.Application.Players;
using Webgame.Infrastructure.Players;
using Microsoft.EntityFrameworkCore;
using Webgame.Infrastructure.Persistence;
using Webgame.Application.Persistence;
using Webgame.Api.Common;
using Microsoft.AspNetCore.Diagnostics;

namespace Webgame.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // For debugging purposes, print the connection string (without the password) to the console.
            var cs = builder.Configuration.GetConnectionString("WebgameDb");
            Console.WriteLine("DB CS (sanitized): " + (cs is null ? "NULL" : cs.Split("Password=")[0]));

            #region Services
            
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Application
            builder.Services.AddScoped<PlayerService>();

            // Infrastructure
            builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
            builder.Services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            // Persistence
            builder.Services.AddDbContext<WebgameDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("WebgameDb")));

            // Common
            builder.Services.AddProblemDetails();
            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            #endregion
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseExceptionHandler();
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
