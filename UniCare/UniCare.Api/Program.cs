using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Microsoft.OpenApi.Models;
using UniCare.Api.Middelware;
using Microsoft.IdentityModel.Tokens;
using UniCare.Application;
using UniCare.Infrastructure;

namespace UniCare.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers();

        // Register application and infrastructure services (ONCE)
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddDirectoryBrowser();

        // Add CORS (MUST be registered here, NOT inside Swagger configuration)
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader());

            // Alternative: More secure policy for production with credentials
            options.AddPolicy("AllowSpecific", policy =>
                policy.WithOrigins("http://localhost:3000", "https://yourdomain.com")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials());
        });

        // Add Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UniCare API",
                Version = "v1",
                Description = "Peer-to-peer marketplace for university students - Authentication & Verification module.",
                Contact = new OpenApiContact { Name = "UniCare Team" }
            });

            // Include XML comments if they exist
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

            // Add JWT authentication to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter your JWT token. Example: Bearer eyJhbG..."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline
        app.UseGlobalExceptionHandler();

        // Enable Swagger in all environments (or conditionally)
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniCare API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "UniCare API Documentation";
            c.DisplayRequestDuration();
        });

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        // Apply CORS policy
        app.UseCors("AllowAll"); // Use "AllowSpecific" for production

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}