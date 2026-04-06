namespace PilotIceService.Application.Events.Common
{
    public abstract class ApplicationEventHandlerBase<TEvent> : IApplicationEventHandler<TEvent>
        where TEvent : ApplicationEvent
    {
        public Task Handle(ApplicationEvent @event, CancellationToken ct)
        {
            return Handle(@event as TEvent, ct);
        }

        public abstract Task Handle(TEvent @event, CancellationToken ct);
    }
}