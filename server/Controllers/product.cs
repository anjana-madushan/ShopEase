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
      var userId = await AuthorizeAdminAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      var products = await _mongoDBService.GetProductsAsync();
      return Ok(products);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("customer")]
  public async Task<IActionResult> GetActiveProducts()
  {
    try
    {
      var userId = await AuthorizeCustomerAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var products = await _mongoDBService.GetProductsCustomerAsync();
      if (products.Count == 0)
      {
        return NotFound(new { Message = "Nothing to show" });
      }
      return Ok(products);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("getCategories")]
  public async Task<IActionResult> GetCategories()
  {
    try
    {
      var userId = await AuthorizeCustomerAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var categories = await _mongoDBService.GetDistinctCategoriesAsync();
      if (categories.Count == 0)
      {
        return NotFound(new { Message = "Nothing to show" });
      }

      return Ok(categories);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("vender")]
  public async Task<IActionResult> GetLoggedVenderProducts()
  {
    try
    {
      var userId = await AuthorizeVenderAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var products = await _mongoDBService.GetProductsVenderAsync(userId);
      if (products.Count == 0)
      {
        return NotFound(new { Message = "Nothing to show" });
      }
      return Ok(products);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("{category}")]
  public async Task<IActionResult> CategoryBasedSearch(string category)
  {
    try
    {
      var userId = await AuthorizeCustomerAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      var products = await _mongoDBService.GetProductsCategoryBasedAsync(category);
      if (products.Count == 0)
      {
        return NotFound(new { Message = "Nothing to show in this category" });
      }
      return Ok(products);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("search")]
  public async Task<IActionResult> Search(string productName)
  {
    try
    {
      var userId = await AuthorizeCustomerAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      if (string.IsNullOrWhiteSpace(productName))
      {
        return BadRequest("Product name cannot be empty.");
      }
      var products = await _mongoDBService.SearchProduct(productName);
      if (products.Count == 0)
      {
        return NotFound(new { Message = "Nothing to show in this category" });
      }
      return Ok(products);
    }
    catch (Exception error)
    {
      return StatusCode(500, new { Message = "An unexpected error occurred while Getting the Products", Error = error.Message });
    }
  }

  [HttpGet("pricesort")]
  public async Task<IActionResult> SortBasedPrice()
  {
    try
    {
      var userId = await AuthorizeCustomerAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      var products = await _mongoDBService.GetProductsCustomerAsync();
      if (products.Count == 0)
      {
        return NotFound(new { Message = "Products are not found" });
      }
      var productSortList = products.OrderByDescending(v => v.Price).ToList();
      return Ok(productSortList);
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

  [HttpGet("customer/{id:length(24)}")]
  public async Task<ActionResult<Product>> GetProductCustomer(string id)
  {
    try
    {
      var userId = await AuthorizeCustomerAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      var product = await _mongoDBService.GetProductAsync(id);

      if (product is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      Console.WriteLine(product.IsActive);

      if (!product.IsActive || !product.IsCategoryActive)
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
      var userId = await AuthorizeVenderAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      // The provided product is valid
      if (!ModelState.IsValid)
      {
        return BadRequest(ModelState);
      }

      var product = new Product
      {
        VenderId = userId,
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
  public async Task<IActionResult> Update(string id, ProductDTO updatedProductDto)
  {
    try
    {
      var userId = await AuthorizeVenderAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      var product = await _mongoDBService.GetProductAsync(id);

      if (product is null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      // Ensure that the product belongs to the authorized vendor
      if (product.VenderId != userId)
      {
        return Unauthorized("Unauthorized vendor.");
      }

      var updatedProduct = new Product
      {
        Id = product.Id,
        VenderId = userId,
        ProductName = updatedProductDto.ProductName,
        Price = updatedProductDto.Price,
        Category = updatedProductDto.Category,
        Description = updatedProductDto.Description,
        IsActive = updatedProductDto.IsActive,
        StockLevel = updatedProductDto.StockLevel,
        MinStockLevel = updatedProductDto.MinStockLevel,
      };

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
      var userId = await AuthorizeVenderAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }

      var product = await _mongoDBService.GetProductAsync(id);

      if (product == null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      // Ensure that the product belongs to the authorized vendor
      if (product.VenderId != userId)
      {
        return Unauthorized("Unauthorized vendor.");
      }

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
      var userId = await AuthorizeVenderAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      var product = await _mongoDBService.GetProductAsync(id);
      if (product == null)
      {
        return NotFound(new { Message = "Product not found" });
      }

      // Ensure that the product belongs to the authorized vendor
      if (product.VenderId != userId)
      {
        return Unauthorized("Unauthorized vendor.");
      }
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

  [HttpPatch("status/{category}")]
  public async Task<IActionResult> ChangeCategoryStatus(string category, [FromBody] bool newStatus)
  {
    try
    {
      var userId = await AuthorizeAdminAsync();
      if (userId == null)
      {
        return Unauthorized("Unauthorized user.");
      }
      var products = await _mongoDBService.GetAllProductsBasedOnCategoryAsync(category);
      if (products.Count == 0)
      {
        return NotFound(new { Message = "Products not found" });
      }

      var updateResult = await _mongoDBService.UpdateCategoryStatusAsync(category, newStatus);

      // Check if any documents were updated
      if (updateResult.ModifiedCount == 0)
      {
        return NotFound(new { Message = "No products were updated." });
      }

      return Ok(new { Message = "Category status updated successfully", updateResult.ModifiedCount });

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

  private async Task<string> AuthorizeVenderAsync()
  {
    // Validate token
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); // Remove "Bearer " prefix
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
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); // Remove "Bearer " prefix
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
    var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", ""); // Remove "Bearer " prefix
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