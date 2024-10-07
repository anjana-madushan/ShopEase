using System;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;

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
  public async Task<List<Product>> Get()
  {
    return await _mongoDBService.GetProductsAsync();
  }

  [HttpGet("{id:length(24)}")]
  public async Task<ActionResult<Product>> Get(string id)
  {
    var product = await _mongoDBService.GetProductAsync(id);

    if (product is null)
    {
      return NotFound();
    }

    return product;
  }

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Product product)
  {
    await _mongoDBService.CreateProductAsync(product);
    return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
  }

  [HttpPut("{id:length(24)}")]
  public async Task<IActionResult> Update(string id, Product updatedBook)
  {
    var product = await _mongoDBService.GetProductAsync(id);

    if (product is null)
    {
      return NotFound();
    }

    updatedBook.Id = product.Id;

    await _mongoDBService.UpdateProductAsync(id, updatedBook);

    return NoContent();
  }

  [HttpDelete("{id}")]
  public async Task<IActionResult> Delete(string id)
  {
    await _mongoDBService.DeleteProductAsync(id);
    return NoContent();
  }

}