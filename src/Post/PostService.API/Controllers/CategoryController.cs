using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;

namespace PostService.API.Controllers;

[ApiController]
[Route("api/{controller}")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;
    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ICategoryService service, ILogger<CategoryController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpGet]
    public async Task Get() => Ok(await _service.Get());

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid? id)
    {
        _logger.LogInformation($"==> A Delete request for the category has been received, ID: {id}");

        if (id is null) return BadRequest();

        // var model = await _service.Get(id.Value);

        // if (model is null) return NotFound();

        await _service.Delete(id.Value);

        return Ok();
    }
}
