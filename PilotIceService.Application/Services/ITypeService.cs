namespace PilotIceService.Application.Services
{
    public interface ITypeService
    {
        Task<object[]> GetAsync(int? type, int? maxResults, CancellationToken ct);
    }
}