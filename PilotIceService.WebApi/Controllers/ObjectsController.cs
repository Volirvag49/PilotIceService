using Microsoft.AspNetCore.Mvc;
using PilotIceService.Application.Services;

namespace PilotIceService.WebApi.Controllers
{
    [ApiController]
    [Tags("Объекты")]
    public class ObjectsController : ControllerBase
    {
        private readonly IObjectService _service;

        /// <inheritdoc />
        public ObjectsController(IObjectService service)
        {
            _service = service;
        }

        /// <summary>
        /// Список объектов.
        /// </summary>
        /// <param name="id">Id.</param>
        /// <param name="parentId">parentId.</param>
        /// <param name="searchString">searchString.</param>
        /// <param name="ct">CancellationToken.</param>
        /// <returns></returns>
        [HttpGet("objects/list")]
        public async Task<IActionResult> GetListAsync(
            [FromQuery] int? type,
            [FromQuery] int? maxResults,
            [FromQuery] Guid? id,
            [FromQuery] Guid? parentId,
            [FromQuery] string? searchString,
            CancellationToken ct)
        {
            var results = await _service.GetAsync(type, maxResults, id, parentId, searchString, ct);
            return Ok(results);
        }
    }
}