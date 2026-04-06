using PilotIceService.Application.Events.Common;

namespace PilotIceService.Application.Events;

public class DepartmentsIsMissingHandler : ApplicationEventHandlerBase<DepartmentsIsMissing>
{

    /// <inheritdoc />
    public DepartmentsIsMissingHandler()
    {

    }

    /// <inheritdoc />
    public override async Task Handle(DepartmentsIsMissing @event, CancellationToken ct)
    {
        await Task.Delay(100);
    }
}