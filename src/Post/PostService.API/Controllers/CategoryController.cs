using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostService.Application.Interfaces;

namespace PostService.API.Controllers;

[ApiController]
[Route("api/{controller}")]
public class CategoryController(ICategoryService service, ILogger<CategoryController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task Get() => Ok(await service.Get());

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid? id)
    {
        logger.LogInformation($"==> A Delete request for the category has been received, ID: {id}");

        if (id is null) return BadRequest();

        var model = await service.Get(id.Value);

        if (model is null) return NotFound();

        await service.Delete(id.Value);

        return Ok();
    }
}
