using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using UniCare.Application.Common.Behaviors;
using MediatR;


namespace UniCare.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = typeof(DependencyInjection).Assembly;

            services.AddMediatR(cfg =>
            {
                cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODA4MTc5MjAwIiwiaWF0IjoiMTc3NjY1NzkxMyIsImFjY291bnRfaWQiOiIwMTlkYTkwZjY4ZmE3MWU5YjYwNjk3NmY2OGU4ODlkYyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxa3BtaDBwMGZncGN4ajF5aGZmd2dqYmU5Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.bhDyaTPmaIIM2a3JnOXYR_cfeaGKLeTJJFDBeOrutD7BtU0cE3Cf9Bnw6lVvDqQxXaABWBRnOttEdca49Uxw-nugbvAEXLbbcm-q3DFX8OYKuH4LbsjtYZRQBQR9TiVOOY9VWM6IE7dtD94eJJGE7NRU4pKRWcV0bNVk6NOYS5VMnDX4LjXFmvNb95TF4lbHMg5yUSWOhaXPMJtn9qSBosGJAZVJRAaoHiJzDWIrEPLX1GM8vtozoSBwzJ5aSUKziP8WVILoiJr_Hr3c3KyFhYBGRZz02KhshkJz3KDDATRh-pD2TcKvY8VpGl5Dbg9BZcgabEBKG-SwL3VPE7fHMg";
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            // Register all FluentValidation validators from this assembly
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }

}
