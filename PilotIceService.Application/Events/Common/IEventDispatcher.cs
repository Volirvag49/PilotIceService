namespace PilotIceService.Application.Events.Common
{
    public interface IEventDispatcher
    {
        Task Dispatch(ApplicationEvent @event, CancellationToken ct);
    }
}