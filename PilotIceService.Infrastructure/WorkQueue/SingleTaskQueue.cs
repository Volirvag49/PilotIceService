namespace PilotIceService.Infrastructure.WorkQueue
{
    public class SingleTaskQueue : IWorkQueue
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);

        public async Task<IDisposable> LockAsync(CancellationToken ct = default)
        {
            await _semaphore.WaitAsync(ct);
            return new SemaphoreReleaser(_semaphore);
        }
    }
}