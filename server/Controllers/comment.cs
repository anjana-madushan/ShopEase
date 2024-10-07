using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using MongoDB.Driver;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using server.DTOs;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
  private readonly MongoDBService _mongoDBService;
  private readonly JwtSettings _jwtSettings;

  public CommentController(MongoDBService mongoDBService, IOptions<JwtSettings> jwtSettings)
  {
    _mongoDBService = mongoDBService;
    _jwtSettings = jwtSettings.Value;
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

  [HttpGet("customer")]
  public async Task<IActionResult> GetCustomerSpecificComments()
  {
    try
    {
      var customerId = await AuthorizeCustomerAsync();
      if (customerId == null)
      {
        return Unauthorized("Unauthorised User");
      }
      var comments = await _mongoDBService.GetCustomerSpecificCommentsAsync(customerId);
      if (comments.Count == 0)
      {
        return NotFound(new { Message = "There aren't any comments to show!!!" });
      }
      return Ok(comments);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("vender")]
  public async Task<IActionResult> GetVenderSpecificComments()
  {
    try
    {
      var venderId = await AuthorizeVenderAsync();
      if (venderId == null)
      {
        return Unauthorized("Unauthorised User");
      }
      var comments = await _mongoDBService.GetVenderSpecificCommentsAsync(venderId);
      if (comments.Count == 0)
      {
        return NotFound(new { Message = "There aren't any comments to show!!!" });
      }
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

  [HttpPost("{venderId:length(24)}")]
  public async Task<IActionResult> Post(string venderId, [FromBody] CommentDto commentDto)
  {
    try
    {
      var customerId = await AuthorizeCustomerAsync();
      if (customerId == null)
      {
        return Unauthorized("Unauthorised User");
      }

      var comment = new Comments
      {
        CustomerId = customerId,
        VendorId = venderId,
        Comment = commentDto.Comment,
        Rating = commentDto.Rating
      };

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

  //Vendor Ranking List Based on Rating 
  [HttpGet("rankings")]
  public async Task<ActionResult<Comments>> GetVendorRankingList()
  {
    try
    {
      var customerId = await AuthorizeCustomerAsync();
      if (customerId == null)
      {
        return Unauthorized("Unauthorised User");
      }
      var comments = await _mongoDBService.GetCommentsAsync();

      if (comments.Count == 0)
      {
        return NotFound(new { Message = "Comments not found" });
      }

      var vendorRatingsList = comments.GroupBy(comment => comment.VendorId).Select(group => new
      {
        VendorId = group.Key,
        AverageRating = group.Average(comment => comment.Rating),
        TotalRatings = group.Count()
      }).OrderByDescending(v => v.AverageRating).ToList();

      var vendors = await _mongoDBService.GetVendorsAsync();
      if (vendors.Count == 0)
      {
        return NotFound(new { Message = "Vendors not found" });
      }

      var vendorRatings = vendorRatingsList
            .Join(vendors,
                rating => rating.VendorId,
                vendor => vendor.Id,
                (rating, vendor) => new
                {
                  rating.VendorId,
                  vendor.Username,
                  rating.AverageRating,
                  rating.TotalRatings
                })
            .ToList();
      if (vendorRatings.Count == 0)
      {
        return NoContent();
      }

      return Ok(vendorRatings);
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

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, CommentDto updatedCommentdto)
  {
    try
    {
      var customerId = await AuthorizeCustomerAsync();
      if (customerId == null)
      {
        return Unauthorized("Unauthorised User");
      }

      var comment = await _mongoDBService.GetCommentAsync(id);

      if (comment is null)
      {
        return NotFound(new { Message = "Comment not found" });
      }

      var updatedComment = new Comments
      {
        Id = comment.Id,
        CustomerId = customerId,
        VendorId = comment.VendorId,
        Comment = updatedCommentdto.Comment,
        Rating = updatedCommentdto.Rating
      };

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
      var customerId = await AuthorizeCustomerAsync();
      if (customerId == null)
      {
        return Unauthorized("Unauthorised User");
      }

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



  private async Task<string> AuthorizeVenderAsync()
  {
    // Validate token
    var token = Request.Headers["Authorization"];
    if (string.IsNullOrEmpty(token))
    {
      return null;
    }

    // Call JWT Service to validate the token
    var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

    // Check if the token is valid
    if (tokenverify == null)
    {
      return null;
    }

    // Check if the user ID is in the token
    var idClaim = tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    if (idClaim == null)
    {
      return null;
    }

    // Check if the user is an vender
    var VenderUser = await _mongoDBService.GetVendorByIdAsync(idClaim.Value);
    if (VenderUser == null)
    {
      return null;
    }

    return idClaim.Value;
  }

  private async Task<string> AuthorizeCustomerAsync()
  {
    // Validate token
    var token = Request.Headers["Authorization"];
    if (string.IsNullOrEmpty(token))
    {
      return null;
    }

    // Call JWT Service to validate the token
    var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

    // Check if the token is valid
    if (tokenverify == null)
    {
      return null;
    }

    // Check if the user ID is in the token
    var idClaim = tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    if (idClaim == null)
    {
      return null;
    }

    // Check if the user is an vender
    var VenderUser = await _mongoDBService.GetCustomerByIdAsync(idClaim.Value);
    if (VenderUser == null)
    {
      return null;
    }

    return idClaim.Value;
  }

  private async Task<string> AuthorizeAdminAsync()
  {
    // Validate token
    var token = Request.Headers["Authorization"];
    if (string.IsNullOrEmpty(token))
    {
      return null;
    }

    // Call JWT Service to validate the token
    var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

    // Check if the token is valid
    if (tokenverify == null)
    {
      return null;
    }

    // Check if the user ID is in the token
    var idClaim = tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
    if (idClaim == null)
    {
      return null;
    }

    // Check if the user is an vender
    var VenderUser = await _mongoDBService.GetAdminByIdAsync(idClaim.Value);
    if (VenderUser == null)
    {
      return null;
    }

    return idClaim.Value;
  }

}