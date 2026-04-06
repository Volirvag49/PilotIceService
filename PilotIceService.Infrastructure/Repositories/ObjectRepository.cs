using PilotIceService.Application.Repositories;
using PilotIceService.Infrastructure.Clients.PilotIce;

namespace PilotIceService.Infrastructure.Repositories
{
    public class ObjectRepository : IObjectRepository
    {
        private readonly PilotClient _pilotClient;

        public ObjectRepository(PilotClient pilotClient)
        {
            _pilotClient = pilotClient;
        }

        /// <inheritdoc />
        public async Task<object[]> GetAsync(int? type, int? maxResults, Guid? id, Guid? parentId, string? searchString, CancellationToken ct)
        {
            var results = await _pilotClient.SearchDObjects(type, maxResults, id, parentId, searchString, ct);

            return results?.ToArray();
        }
    }
}