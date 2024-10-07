using server.Models;
using System;

namespace server.Services
{
  public class StockLimitAlert
  {

    private readonly EmailService _emailService;

    // Constructor to inject EmailService
    public StockLimitAlert(EmailService emailService)
    {
      _emailService = emailService;
    }

    public async Task<string> TriggerLowStockAlert(Product product, int updatedStock, string email)
    {
      string alertMessage = $"Low stock alert for product {product.ProductName}. Current stock: {updatedStock}";

      await _emailService.SendEmailAsync(
        email,
        subject: "Low Stock Alert",
        alertMessage
      );

      return alertMessage;
    }
  }
}
