using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PilotIceService.Application.Repositories;
using PilotIceService.Infrastructure.Clients.PilotIce;
using PilotIceService.Infrastructure.Kafka;
using PilotIceService.Infrastructure.Repositories;
using PilotIceService.Infrastructure.WorkQueue;

namespace PilotIceService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPilotIceClient(configuration);
            services.AddWorkQueue();
            services.AddKafkaQueue(configuration);
        
            services.AddScoped<ITypeRepository, TypeRepository>();
            services.AddScoped<IObjectRepository, ObjectRepository>();
            return services;
        }
    }
}