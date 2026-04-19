using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Microsoft.OpenApi.Models;
using UniCare.Api.Middelware;
using UniCare.Application;
using Microsoft.IdentityModel.Tokens;
using UniCare.Application;
using UniCare.Infrastructure;

namespace UniCare.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddControllers();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);

        builder.Services.AddDirectoryBrowser();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UniCare API",
                Version = "v1",
                Description = "Peer-to-peer marketplace for university students � Authentication & Verification module.",
                Contact = new OpenApiContact { Name = "UniCare Team" }
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

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);

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
                            Id   = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });


        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
                policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        });


        var app = builder.Build();


        app.UseGlobalExceptionHandler();

        //if (app.Environment.IsDevelopment())
        //{
        //    app.UseSwagger();
        //    app.UseSwaggerUI();// c =>
        //    //{
        //    //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniCare API v1");
        //    //    c.RoutePrefix = string.Empty;
        //    //});
        //}
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "UniCare API v1");
            c.RoutePrefix = "swagger"; // Explicitly set route prefix

            // Optional: Customize for production
            c.DocumentTitle = "UniCare API Documentation";
            c.DisplayRequestDuration();
        });

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseCors("AllowAll");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();


        app.Run();
    }
}