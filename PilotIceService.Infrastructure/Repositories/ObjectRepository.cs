using PilotIceService.Application.Repositories;
using PilotIceService.Infrastructure.Clients.PilotIce;
using PilotIceService.Infrastructure.WorkQueue;

namespace PilotIceService.Infrastructure.Repositories
{
    public class ObjectRepository : IObjectRepository
    {

        private readonly PilotClient _pilotClient;
        private readonly IWorkQueue _queue;

        public ObjectRepository(PilotClient pilotClient, IWorkQueue queue)
        {
            _pilotClient = pilotClient;
            _queue = queue;
        }

        /// <inheritdoc />
        public async Task<object[]> GetAsync(int? type, int? maxResults, Guid? id, Guid? parentId, string? searchString, CancellationToken ct)
        {
            using var locker = await _queue.LockAsync(ct);
            var results = await _pilotClient.SearchDObjects(type, maxResults, id, parentId, searchString, ct);
            return results?.ToArray();
        }
    }
}