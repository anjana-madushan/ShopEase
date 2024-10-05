using System;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using MongoDB.Driver;

namespace MongoExample.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{

  private readonly MongoDBService _mongoDBService;

  public ProductController(MongoDBService mongoDBService)
  {
    _mongoDBService = mongoDBService;
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
  public async Task<IActionResult> Post([FromBody] Product product)
  {
    try
    {
      await _mongoDBService.CreateProductAsync(product);
      return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
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
  public async Task<IActionResult> Update(string id, Product updatedProduct)
  {
    try
    {
      var product = await _mongoDBService.GetProductAsync(id);

      if (product is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      updatedProduct.Id = product.Id;

      await _mongoDBService.UpdateProductAsync(id, updatedProduct);

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