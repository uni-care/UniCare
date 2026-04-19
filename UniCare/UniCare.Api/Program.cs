using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using UniCare.Application;
using UniCare.Infrastructure;

namespace UniCare.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new() { Title = "UniCare API", Version = "v1" });
            });

            // CORS is required for SignalR WebSocket handshake from a browser client.
            // Tighten AllowedOrigins before deploying to production.
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .SetIsOriginAllowed(_ => true));
            });

            // Clean Architecture layers
            builder.Services.AddApplication();                          // MediatR + Validators + Pipeline
            builder.Services.AddInfrastructure(builder.Configuration); // EF Core + Repos + Services + SignalR

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors();
            app.UseAuthorization();
            app.MapControllers();

            // Maps /hubs/chat SignalR endpoint
            app.UseInfrastructure();

            app.Run();
        }
    }
}