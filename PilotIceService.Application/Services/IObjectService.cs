namespace PilotIceService.Application.Services
{
    public interface IObjectService
    {
        Task<object[]> GetAsync(int? type, int? maxResults, Guid? id, Guid? parentId, string? searchString, CancellationToken ct);
    }
}