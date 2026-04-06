using Microsoft.Extensions.DependencyInjection;
using PilotIceService.Application.Events;
using PilotIceService.Application.Events.Common;
using PilotIceService.Application.Services;

namespace PilotIceService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<ITypeService, TypeService>();

            services.AddScoped<IObjectService, ObjectService>();

            services.AddScoped<IEventDispatcher, ApplicationEventDispatcher>();

            services.AddScoped<IApplicationEventHandler<DepartmentsIsMissing>, DepartmentsIsMissingHandler>();

            return services;
        }
    }
}