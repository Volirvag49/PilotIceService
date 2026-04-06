using Microsoft.AspNetCore.Mvc;
using PilotIceService.Application.Services;

namespace PilotIceService.WebApi.Controllers
{
    [ApiController]
    [Tags("Типы объектов")]
    public class TypesController : ControllerBase
    {
        private readonly ITypeService _service;

        /// <inheritdoc />
        public TypesController(ITypeService service)
        {
            _service = service;
        }

        /// <summary>
        /// Список типов объектов.
        /// </summary>
        /// <param name="ct">CancellationToken.</param>
        /// <returns></returns>
        [HttpGet("types/list")]
        public async Task<IActionResult> GetAsync(
            [FromQuery] int? type,
            [FromQuery] int? maxResults,
            CancellationToken ct)
        {
            var results = await _service.GetAsync(type, maxResults, ct);
            return Ok(results);
        }
    }
}