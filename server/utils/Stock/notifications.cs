using server.Models;
using System;

namespace server.Utils
{
  public class StockAlertUtil
  {

    private readonly EmailServiceAlt _emailService;

    // Constructor to inject EmailService
    public StockAlertUtil(EmailServiceAlt emailService)
    {
      _emailService = emailService;
    }

    public async Task<string> TriggerLowStockAlert(Product product, int updatedStock)
    {
      Console.WriteLine($"Low stock alert for product {product.ProductName}. Current stock: {updatedStock}");
      string alertMessage = $"Low stock alert for product {product.ProductName}. Current stock: {updatedStock}";

      await _emailService.SendEmailAsync(
        to: "pbapmadushan@gmail.com",
        subject: "Low Stock Alert",
        body: alertMessage
      );

      return alertMessage;
    }
  }
}
