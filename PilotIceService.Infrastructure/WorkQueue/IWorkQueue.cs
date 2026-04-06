namespace PilotIceService.Infrastructure.WorkQueue
{
    public interface IWorkQueue
    {
        Task<IDisposable> LockAsync(CancellationToken ct = default);
    }
}