using PilotIceService.Application.Repositories;
using PilotIceService.Infrastructure.Clients.PilotIce;
using PilotIceService.Infrastructure.WorkQueue;

namespace PilotIceService.Infrastructure.Repositories
{
    public class TypeRepository : ITypeRepository
    {
        private readonly PilotClient _pilotClient;
        private readonly IWorkQueue _queue;
        public TypeRepository(PilotClient pilotClient, IWorkQueue queue)
        {
            _pilotClient = pilotClient;
            _queue = queue;
        }

        /// <param name="ct"></param>
        /// <inheritdoc />
        public async Task<object[]> GetAsync(int? type, int? maxResults, CancellationToken ct)
        {
            using var locker = await _queue.LockAsync(ct);
            var results = await _pilotClient.GetTypesAsync(type, maxResults, ct);
            return results;
        }
    }
}