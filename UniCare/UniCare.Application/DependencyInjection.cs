using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniCare.Application.Common.Behaviors;
using FluentValidation;


namespace UniCare.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            // Register all IRequestHandlers in this assembly automatically
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));

            // Register all FluentValidation validators in this assembly
            services.AddValidatorsFromAssembly(assembly);

            // Plug in the validation pipeline — runs before every handler
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
