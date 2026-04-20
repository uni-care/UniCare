using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using UniCare.Api.Middelware;
using UniCare.Application;
using UniCare.Domain.Repositories;
using UniCare.Infrastructure;
using UniCare.Infrastructure.Persistence.Repositories;

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
        builder.Services.AddScoped<IItemRepository, ItemRepository>();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("MyCorsPolicy", policy =>
            {
            policy.WithOrigins(
                    "https://uni-care-front.vercel.app/",
                    "https://localhost:3000",
                    "http://localhost:3000"
                  )
                  .SetIsOriginAllowedToAllowWildcardSubdomains()
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });

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
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
     

        var app = builder.Build();

        app.UseGlobalExceptionHandler();

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

        app.UseCors("AllowAll"); 

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}