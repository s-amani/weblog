using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PostService.Application.DTOs;
using PostService.Application.Interfaces;

namespace PostService.API.Controllers;

[ApiController]
[Route("api/{controller}")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly ILogger<PostController> _logger;

    public PostController(IPostService postService, ILogger<PostController> logger)
    {
        _postService = postService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult> Post(PostCreateDTO postDTO)
    {
        var model = await _postService.Create(postDTO);
        return CreatedAtAction(nameof(Get), new {Id = model.Id }, model);
    }
    
    [HttpGet]
    public async Task<ActionResult> Get() 
    {
        var model = await _postService.Get();
        
        if (model is null)
            return NotFound();

        return Ok(model);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(Guid? id) 
    {
        _logger.LogInformation($"==> A Get request has been received, ID: {id}");

        if (!id.HasValue)
            return BadRequest();

        var model = await _postService.Get(id.Value);
        
        if (model is null)
            return NotFound();

        return Ok(model);
    }
}
