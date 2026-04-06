using PilotIceService.Application.Repositories;

namespace PilotIceService.Application.Services
{
    public class TypeService : ITypeService
    {
        private readonly ITypeRepository _repository;

        public TypeService(ITypeRepository repository)
        {
            _repository = repository;
        }

        /// <inheritdoc />
        public async Task<object[]> GetAsync(int? type, int? maxResults, CancellationToken ct)
        {
            return await _repository.GetAsync(type, maxResults, ct);
        }
    }
}