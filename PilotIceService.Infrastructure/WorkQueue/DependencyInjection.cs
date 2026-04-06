using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PilotIceService.Infrastructure.WorkQueue
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWorkQueue(this IServiceCollection services)
        {
            services.AddSingleton<IWorkQueue, SingleTaskQueue>();
            return services;
        }
    }
}