using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using System.Threading.Tasks;

public class EmailService
{
  private readonly EmailSettings _emailSettings;

  public EmailService(IOptions<EmailSettings> emailSettings)
  {
    _emailSettings = emailSettings.Value;
  }
  public async Task SendEmailAsync(string to, string subject, string body)
  {
    var message = new MimeMessage();
    message.From.Add(new MailboxAddress("Shop_Ease", _emailSettings.FromEmail));
    message.To.Add(new MailboxAddress("", to));
    message.Subject = subject;
    message.Body = new TextPart("html") { Text = body };

    using (var client = new SmtpClient())
    {
      await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, SecureSocketOptions.StartTls);
      await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
      await client.SendAsync(message);
      await client.DisconnectAsync(true);
    }
  }
}
