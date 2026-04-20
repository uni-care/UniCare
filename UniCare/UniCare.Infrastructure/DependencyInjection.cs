using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using UniCare.Domain.Aggregates.ChatAggregate;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;
using UniCare.Infrastructure.Hubs;
using UniCare.Infrastructure.Persistence;
using UniCare.Infrastructure.Repositories;
using UniCare.Infrastructure.Services;
﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Interfaces;
using UniCare.Domain.Aggregates.ChatAggregate;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;
using UniCare.Domain.Aggregates.UserAggregates;
using UniCare.Domain.Interfaces;
using UniCare.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using UniCare.Infrastructure.services.Ocr;



namespace UniCare.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<UniCareDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.MigrationsAssembly(typeof(UniCareDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider =>
                provider.GetRequiredService<UniCareDbContext>());
            services.AddHttpClient<IAiRecommendationService, AiRecommendationService>(client => {
                client.BaseAddress = new Uri(configuration["AiEndpoint:BaseUrl"]);
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {configuration["AiEndpoint:Key"]}");
            });
            services.AddIdentity<User, IdentityRole<Guid>>(options =>
            services.AddCors(options =>
            {
                options.AddPolicy("SignalRCors", policy =>
                {
                    policy.WithOrigins("http://localhost:3000", "https://uni-care-front.vercel.app/") // Add your frontend URLs
                          .AllowAnyMethod()
                          .AllowAnyHeader()
                          .AllowCredentials(); 
                });
            });

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;

                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<UniCareDbContext>()
            .AddDefaultTokenProviders();

            var jwtSection = configuration.GetSection(JwtSettings.SectionName);
            services.Configure<JwtSettings>(jwtSection);

            var jwtSettings = jwtSection.Get<JwtSettings>()!;
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) &&
                            path.StartsWithSegments("/hubs/chat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            var ocrSection = configuration.GetSection(OcrSettings.SectionName);
            services.Configure<OcrSettings>(ocrSection);

            var ocrSettings = ocrSection.Get<OcrSettings>() ?? new OcrSettings();

            services.AddHttpClient<IOcrService, RealOcrService>();
      
            services.AddScoped<ITransactionHandoverRepository, TransactionHandoverRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();

            services.AddSingleton<IPinGeneratorService, PinGeneratorService>();

            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ISignInService, SignInService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IChatNotificationService, SignalRChatNotificationService>();

            services.AddSignalR();

            return services;
        }

        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.UseCors("SignalRCors");

            app.MapHub<ChatHub>("/hubs/chat");
            return app;
        }
    }
}