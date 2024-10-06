using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using MongoDB.Driver;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{

  private readonly MongoDBService _mongoDBService;

  public CommentController(MongoDBService mongoDBService)
  {
    _mongoDBService = mongoDBService;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    try
    {
      var comments = await _mongoDBService.GetCommentsAsync();
      return Ok(comments);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Comments>> Get(string id)
  {
    try
    {
      var comment = await _mongoDBService.GetCommentAsync(id);

      if (comment is null)
      {
        return NotFound(new { Message = "Comment and Rate not found" });
      }

      return Ok(comment);

    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Mongo DB error occurred while getting this comment", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Comments comment)
  {
    try
    {
      await _mongoDBService.CreateCommentAndRate(comment);
      return CreatedAtAction(nameof(Get), new { id = comment.Id }, comment);
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Error ocurred when adding the details to the MongoDB", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }


  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, Comments updatedComment)
  {
    try
    {
      var comment = await _mongoDBService.GetCommentAsync(id);

      if (comment is null)
      {
        return NotFound(new { Message = "Comment not found" });
      }

      updatedComment.Id = comment.Id;

      await _mongoDBService.UpdateCommentAsync(id, updatedComment);

      return Ok(new { Message = "Comment updated successfully", UpdatedComment = updatedComment });
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Mongo DB error occurred while updating this comment", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }


  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id)
  {
    try
    {
      var isRemoved = await _mongoDBService.DeleteCommentAsync(id);
      if (!isRemoved)
      {
        return NotFound(new { Message = "Comment and Rate not found" });
      }
      return Ok(new { Message = "Comment and Rate deleted successfully" });
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Mongo DB error occurred while deleting this comment", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }

  }

}