namespace PilotIceService.Infrastructure.WorkQueue
{
    public sealed class SemaphoreReleaser : IDisposable
    {
        private readonly SemaphoreSlim _semaphore;

        public SemaphoreReleaser(SemaphoreSlim semaphore)
        {
            _semaphore = semaphore;
        }

        public void Dispose()
        {
            _semaphore.Release();
        }
    }
}