namespace PilotIceService.Application.Events.Common
{
    public interface IApplicationEventHandler<in TEvent> : IApplicationEventHandler where TEvent : ApplicationEvent
    {
        Task Handle(TEvent @event, CancellationToken ct);
    }

    public interface IApplicationEventHandler
    {
        Task Handle(ApplicationEvent @event, CancellationToken ct);
    }
}