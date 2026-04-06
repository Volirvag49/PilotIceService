namespace PilotIceService.Application.Repositories
{
    public interface IObjectRepository
    {
        Task<object[]> GetAsync(int? type, int? maxResults, Guid? id, Guid? parentId, string? searchString, CancellationToken ct);
    }
}