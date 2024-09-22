using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PostService.Application.DTOs.Comment;
using PostService.Application.Interfaces;

namespace PostService.API.Controllers;

[ApiController]
[Route("api/posts/{postId}/comments")]
public class CommentController : ControllerBase
{
    private readonly IPostService _service;

    public CommentController(IPostService service)
    {
        _service = service;
    }
    
    [HttpPost]
    public async Task<IActionResult> Post(Guid postId, [FromBody]CommentCreateDTO comment)
    {
        await _service.AddCommentToPostAsync(postId, comment);
        return Ok();
    }

    [HttpPut("{commentId}")]
    public async Task<IActionResult> Put(Guid postId, Guid commentId, CommentUpdateDTO comment)
    {
        await _service.UpdateCommentAsync(postId, commentId, comment);
        return Ok();
    }

    [HttpDelete("{commentId}")]
    public async Task<IActionResult> Delete(Guid postId, Guid commentId)
    {
        await _service.DeleteCommentAsync(postId, commentId);
        return Ok();
    }
}
