using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniCare.Domain.Aggregates.ChatAggregate;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandoverAggregate;
using UniCare.Infrastructure.Hubs;
using UniCare.Infrastructure.Persistence;
using UniCare.Infrastructure.Repositories;
using UniCare.Infrastructure.Services;

namespace UniCare.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<UniCareDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ITransactionHandoverRepository, TransactionHandoverRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();

            services.AddSingleton<IPinGeneratorService, PinGeneratorService>();

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