using System;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using MongoDB.Driver;
using server.DTOs;
using System.Security.Claims;
using Microsoft.Extensions.Options;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{

  private readonly MongoDBService _mongoDBService;
  private readonly JwtSettings _jwtSettings;

  public ProductController(MongoDBService mongoDBService, IOptions<JwtSettings> jwtSettings)
  {
    _mongoDBService = mongoDBService;
    _jwtSettings = jwtSettings.Value;
  }

  [HttpGet]
  public async Task<IActionResult> Get()
  {
    try
    {
      var products = await _mongoDBService.GetProductsAsync();
      return Ok(products);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Product>> Get(string id)
  {
    try
    {
      var product = await _mongoDBService.GetProductAsync(id);

      if (product is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      return Ok(product);

    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Mongo DB error occurred while getting this product", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] ProductDTO productDto)
  {
    try
    {
      // Validate token
      var token = Request.Headers["Authorization"];
      if (token.Count == 0)
      {
        return Unauthorized("No token provided.");
      }

      // Call JWT Service to validate the token
      var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

      // Check if the token is valid
      if (tokenverify == null)
      {
        return Unauthorized("Invalid token.");
      }

      // Check if the email is in the token
      var emailClaim = tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
      if (emailClaim == null)
      {
        return Unauthorized("Invalid token.");
      }

      // Check if the user is an vendor
      var adminUser = await _mongoDBService.GetVendorByEmailAsync(emailClaim.Value);
      if (adminUser == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var venderList = await _mongoDBService.GetVendorByIdAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

      // The provided product is valid
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var product = new Product
      {
        ProductName = productDto.ProductName,
        Price = productDto.Price,
        Category = productDto.Category,
        Description = productDto.Description,
        IsActive = productDto.IsActive,
        StockLevel = productDto.StockLevel,
        MinStockLevel = productDto.MinStockLevel
      };


      // Create the product in the database
      await _mongoDBService.CreateProductAsync(product);

      venderList.ProductsCreated.Add(product);
      await _mongoDBService.UpdateVendorAsync(venderList.Id, venderList);

      return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Error occurred when adding the details to the MongoDB", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, Product updatedProduct)
  {
    try
    {
      // Validate token
      var token = Request.Headers["Authorization"];
      if (token.Count == 0)
      {
        return Unauthorized("No token provided.");
      }

      // Call JWT Service to validate the token
      var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

      // Check if the token is valid
      if (tokenverify == null)
      {
        return Unauthorized("Invalid token.");
      }

      // Check if the email is in the token
      var emailClaim = tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
      if (emailClaim == null)
      {
        return Unauthorized("Invalid token.");
      }

      // Check if the user is an vendor
      var adminUser = await _mongoDBService.GetVendorByEmailAsync(emailClaim.Value);
      if (adminUser == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var venderList = await _mongoDBService.GetVendorByIdAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

      var product = await _mongoDBService.GetProductAsync(id);

      if (product is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      updatedProduct.Id = product.Id;

      await _mongoDBService.UpdateProductAsync(id, updatedProduct);

      await _mongoDBService.UpdateVendorAsync(venderList.Id, venderList);

      return Ok(new { Message = "Product updated successfully", UpdatedProduct = updatedProduct });
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Mongo DB error occurred while updating this product", Error = mongoerror.Message });
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
      var isRemoved = await _mongoDBService.DeleteProductAsync(id);
      if (!isRemoved)
      {
        return NotFound(new { Message = "Product not found" });
      }
      return Ok(new { Message = "Product deleted successfully" });
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Mongo DB error occurred while deleting this product", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }

  }


  [HttpPatch("{id}/status")]
  public async Task<IActionResult> ChangeProductStatus(string id, [FromBody] bool newStatus)
  {
    try
    {
      var isPatched = await _mongoDBService.ChangeProductStatusAsync(id, newStatus);
      if (!isPatched)
      {
        return NotFound(new { Message = "Product not found" });
      }
      return Ok(new { Message = "Product Listing status updated successfully." });
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "Mongo DB error occurred while updating the product listing status", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }


}