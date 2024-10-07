using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
  private readonly MongoDBService _mongoDBService;
  private readonly EmailService _emailService;
  private readonly JwtSettings _jwtSettings;

  public InventoryController(MongoDBService mongoDBService, EmailService emailService, IOptions<JwtSettings> jwtSettings)
  {
    _mongoDBService = mongoDBService;
    _emailService = emailService;
    _jwtSettings = jwtSettings.Value;
  }

  [HttpGet("{id}")]
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

  [HttpGet("low-stock-products")]
  public async Task<ActionResult<int>> GetLowStockProducts()
  {
    try
    {
      var userId = await AuthorizeVenderAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var products = await _mongoDBService.GetProductsVenderAsync(userId);
      if (products is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      var lowStockProducts = products.Where(p => p.StockLevel <= p.MinStockLevel).ToList();
      if (lowStockProducts.Count == 0)
      {
        return NotFound(new { Message = "No low stock products found." });
      }

      return Ok(lowStockProducts);
    }
    catch (MongoException mongoerror)
    {
      return StatusCode(500, new { Message = "MongoDB error occurred while retrieving the products", Error = mongoerror.Message });
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
      var userId = await AuthorizeVenderAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var user = await _mongoDBService.GetVendorByIdAsync(userId);

      if (user == null)
      {
        return NotFound(new { Message = "User not found" });
      }

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

      if (updateStock <= product.MinStockLevel)
      {
        var stockAlert = new StockLimitAlert(_emailService);
        await stockAlert.TriggerLowStockAlert(product, updateStock, user.Email);

        var notification = await _mongoDBService.CreateNotification(new Notification
        {
          Message = $"Low stock alert: '{product.ProductName}' is below minimum level. Current stock: {updateStock}.",
          Date = DateTime.Now,
          Read = false,
          UserId = user.Id
        });
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
}

