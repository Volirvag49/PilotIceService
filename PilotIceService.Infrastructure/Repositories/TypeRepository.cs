using PilotIceService.Application.Repositories;
using PilotIceService.Infrastructure.Clients.PilotIce;

namespace PilotIceService.Infrastructure.Repositories
{
    public class TypeRepository : ITypeRepository
    {
        private readonly PilotClient _pilotClient;

        public TypeRepository(PilotClient pilotClient)
        {
            _pilotClient = pilotClient;
        }

        /// <param name="ct"></param>
        /// <inheritdoc />
        public async Task<object[]> GetAsync(int? type, int? maxResults, CancellationToken ct)
        {
            var results = await _pilotClient.GetTypesAsync(type, maxResults, ct);
            return results;
        }
    }
}