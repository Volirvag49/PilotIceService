
using Mappify;
using PilotIceService.Application;
using PilotIceService.Infrastructure;
using PilotIceService.WebApi.Middleware.GlobalExceptions;

namespace PilotIceService.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMappify();
            builder.Services.AddMappifyProfileForAssembly(typeof(Program));
            builder.Services.AddMappifyProfileForAssembly(typeof(Infrastructure.DependencyInjection));
            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddApplication();
            builder.Services.AddInfrastructure(builder.Configuration);

            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionsMiddleware>();

            // Configure the HTTP request pipeline.
            app.MapOpenApi();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/openapi/v1.json", "v1");
            });

            app.MapControllers();

            app.Run();
        }
    }
}
