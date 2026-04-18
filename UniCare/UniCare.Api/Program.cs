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

            // Clean Architecture layers
            builder.Services.AddApplication();                          // MediatR + Validators + Pipeline
            builder.Services.AddInfrastructure(builder.Configuration); // EF Core + Repos + Domain Services

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}
