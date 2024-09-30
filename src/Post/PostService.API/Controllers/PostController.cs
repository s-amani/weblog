using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs.Post;
using PostService.Application.Interfaces;

namespace PostService.API.Controllers;

[ApiController]
[Route("api/{controller}")]
public class PostController(IPostService postService, ILogger<PostController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> Post(PostCreateDTO postDto)
    {
        var model = await postService.Create(postDto);
        return CreatedAtAction(nameof(Get), new {Id = model.Id }, model);
    }
    
    [HttpPut]
    public async Task<ActionResult> Put(Guid? id, PostUpdateDTO postDto)
    {
        if (id is null) return BadRequest();

        var model = await postService.Get(id.Value);

        if (model is null) return NotFound();

        await postService.Update(id.Value, postDto);

        return Ok();
    }
    
    [HttpGet]
    public async Task<ActionResult> Get() 
    {
        var model = await postService.Get();
        
        if (model is null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid? id) 
    {
        logger.LogInformation($"==> A Get request has been received, ID: {id}");

        if (!id.HasValue)
            return BadRequest();

        var model = await postService.Get(id.Value);
        
        if (model is null)
            return NotFound();

        return Ok(model);
    }

    [HttpPost]
    [Route("ChangeStatus")]
    public async Task<ActionResult> ChangeStatus(Guid? id)
    {
        if (id is null) return BadRequest();

        var model = await postService.Get(id.Value);

        if (model is null) return NotFound();

        await postService.ChangePublishStatus(id.Value);

        return Ok();
    }
}
