namespace PilotIceService.Application.Repositories
{
    public interface ITypeRepository
    {
        Task<object[]> GetAsync(int? type, int? maxResults, CancellationToken ct);
    }
}