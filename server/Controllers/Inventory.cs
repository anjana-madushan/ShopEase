using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using MongoDB.Driver;
using server.Utils;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
  private readonly MongoDBService _mongoDBService;
  private readonly EmailService _emailService;

  public InventoryController(MongoDBService mongoDBService, EmailService emailService)
  {
    _mongoDBService = mongoDBService;
    _emailService = emailService;
  }

  [HttpGet("{id}/inventory")]
  public async Task<ActionResult<int>> GetStockLevel(string id)
  {
    try
    {
      var product = await _mongoDBService.GetProductAsync(id);

      if (product is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      return Ok(new { product.StockLevel });
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "MongoDB error occurred while retrieving the product", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }

  // Deduct Stock from a Product
  [HttpPatch("{id}/deduct-stock")]
  public async Task<IActionResult> DeductStock(string id, [FromBody] int quantity)
  {
    try
    {
      var product = await _mongoDBService.GetProductAsync(id);
      if (product is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      // Deduct stock if enough is available
      if (product.StockLevel < quantity)
      {
        return BadRequest(new { Message = "Insufficient stock available" });
      }

      var updateStock = product.StockLevel - quantity;
      await _mongoDBService.DeductAvailableStock(id, updateStock);

      // Check for low stock alert
      Console.WriteLine(product.StockLevel);
      Console.WriteLine(updateStock);
      if (updateStock <= product.MinStockLevel)
      {
        var stockAlertUtil = new StockAlertUtil(_emailService);
        await stockAlertUtil.TriggerLowStockAlert(product, updateStock);
      }

      return Ok(new { Message = "Stock deducted successfully", RemainingStock = updateStock });
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "MongoDB error occurred while processing stock deduction", Error = mongoerror.Message });
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred", Error = error.Message });
    }
  }
}
