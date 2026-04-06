using Microsoft.Extensions.DependencyInjection;

namespace PilotIceService.Application.Events.Common
{
    public class ApplicationEventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public ApplicationEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Dispatch(ApplicationEvent @event, CancellationToken ct)
        {
            var handlerType = typeof(IApplicationEventHandler<>).MakeGenericType(@event.GetType());
            var handlers = _serviceProvider.GetServices(handlerType);
            foreach (var handler in handlers)
            {
                if (handler is IApplicationEventHandler baseHandler)
                {
                    await baseHandler.Handle(@event, ct);
                }
            }
        }
    }
}