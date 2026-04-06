using PilotIceService.Application.Repositories;

namespace PilotIceService.Application.Services
{
    public class ObjectService : IObjectService
    {
        private readonly IObjectRepository _repository;

        public ObjectService(IObjectRepository repository)
        {
            _repository = repository;
        }

        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <param name="searchString"></param>
        /// <inheritdoc />
        public async Task<object[]> GetAsync(int? type, int? maxResults, Guid? id, Guid? parentId, string? searchString, CancellationToken ct)
        {
            return await _repository.GetAsync(type, maxResults, id, parentId, searchString, ct);
        }
    }
}