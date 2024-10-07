using Microsoft.Extensions.Caching.Memory;
using server.Services;
using System;

public class OTPService
{
  private IMemoryCache _cache;
  private EmailService _emailService;

  public OTPService(IMemoryCache cache, EmailService emailService)
  {
    _cache = cache;
    _emailService = emailService;
  }

  public async Task SendOTPAsync(string email)
  {
    try
    {
      var otp = GenerateOTP();
      var cacheEntryOptions = new MemoryCacheEntryOptions()
          .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
      _cache.Set(email, new OTPDetails { Code = otp, Expiry = DateTime.UtcNow.AddMinutes(30) }, cacheEntryOptions);

      await _emailService.SendEmailAsync(email, "Reset Password OTP", $"Your OTP is {otp}. It will expire in 15 minutes.Please change you password within 30 minutes of receiving this email.");
    }
    catch (Exception ex)
    {
      Console.WriteLine($"An error occurred while sending OTP: {ex.Message}");
      throw;
    }
  }

  public bool ValidateOTP(string email, string providedOTP)
  {
    if (_cache.TryGetValue<OTPDetails>(email, out var otpDetails))
    {
      // Check both OTP code match and whether the current time is less than the expiry time
      return otpDetails.Code == providedOTP && DateTime.UtcNow <= otpDetails.Expiry;
    }
    return false;
  }

  public bool ValidateOTPOnReset(string email, string providedOTP)
  {
    if (_cache.TryGetValue<OTPDetails>(email, out var otpDetails))
    {
      bool isValidOTP = otpDetails.Code == providedOTP && DateTime.UtcNow <= otpDetails.Expiry;
      if (isValidOTP)
      {
        _cache.Remove(email);
      }
      return isValidOTP;
    }
    return false;
  }



  private string GenerateOTP(int length = 6)
  {
    var random = new Random();
    return new string(Enumerable.Range(0, length).Select(x => (char)('0' + random.Next(0, 10))).ToArray());
  }
}

public class OTPDetails
{
  public string Code { get; set; }
  public DateTime Expiry { get; set; }
}
