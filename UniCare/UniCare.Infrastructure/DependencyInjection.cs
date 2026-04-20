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
using UniCare.Infrastructure.Hubs;
using UniCare.Infrastructure.Persistence;
using UniCare.Infrastructure.Persistence;
using UniCare.Infrastructure.Repositories;
using UniCare.Infrastructure.services;
using UniCare.Infrastructure.Services;
using UniCare.Infrastructure.Services;
using UniCare.Infrastructure.Settings;



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
            {
                // Password policy
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;

                // Lockout policy
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
                    ClockSkew = TimeSpan.Zero // No tolerance on expiry
                };
            });



            services.AddScoped<ITransactionHandoverRepository, TransactionHandoverRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();

            services.AddSingleton<IPinGeneratorService, PinGeneratorService>();
            // ── Custom Services ───────────────────────────────────────────────────
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<ISignInService, SignInService>();
            services.AddScoped<IOcrService, MockOcrService>();   // Swap for real implementation
            services.AddScoped<IFileStorageService, FileStorageService>();

            services.AddScoped<IChatNotificationService, SignalRChatNotificationService>();

            services.AddSignalR();

            return services;
        }

        public static WebApplication UseInfrastructure(this WebApplication app)
        {
            app.MapHub<ChatHub>("/hubs/chat");
            return app;
        }
    }
}