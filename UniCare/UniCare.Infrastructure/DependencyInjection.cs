using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Domain.Aggregates.TransactionAggregate;
using UniCare.Domain.Aggregates.TransactionHandover;
using UniCare.Domain.Repositories;
using UniCare.Infrastructure.Persistence;
using UniCare.Infrastructure.Persistence.Repositories;
using UniCare.Infrastructure.Repositories;

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
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<ITransactionHandoverRepository, TransactionHandoverRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddSingleton<IPinGeneratorService, PinGeneratorService>();

            return services;
        }
    }
}
