using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace server.Services
{
  public class EmailService
  {
    private readonly SendGridClient _client;
    private readonly EmailAddress _from;

    public EmailService(string apiKey, string senderEmail, string senderName)
    {
      _client = new SendGridClient(apiKey);
      _from = new EmailAddress(senderEmail, senderName);
    }

    public async Task SendEmailAsync(string recipientEmail, string subject, string message)
    {
      try
      {
        var to = new EmailAddress(recipientEmail);
        var plainTextContent = message;

        // Ensure this URL points to where your image is hosted publicly
        var imageUrl = "https://drive.google.com/file/d/1j8rmvrv_k1PlisQwaSWy7Sj-ZnmpA0eP/view?usp=sharing";
        var htmlContent = $"<strong>{message}</strong><br/><img src='{imageUrl}' alt='ShopEase' />";

        var msg = MailHelper.CreateSingleEmail(_from, to, subject, plainTextContent, htmlContent);

        var response = await _client.SendEmailAsync(msg);
        Console.WriteLine($"Response status code: {response.StatusCode}");
        Console.WriteLine($"Response body: {await response.Body.ReadAsStringAsync()}");
        if (!response.IsSuccessStatusCode)
        {
          Console.WriteLine("Failed to send email.");
        }
      }
      catch (Exception ex)
      {
        Console.WriteLine($"An error occurred while sending email: {ex.Message}");
        throw;
      }
    }
  }
}
