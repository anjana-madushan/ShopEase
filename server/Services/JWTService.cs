using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using server.Models;

public class JWTService
{
    public static string GenerateToken(dynamic user, string securityKey, int expirationInMinutes)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(securityKey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            }),
            Expires = DateTime.UtcNow.AddMinutes(expirationInMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = "http://localhost:5147",
            Audience = "http://localhost:5147"
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    // Validate JWT token and return ClaimsPrincipal (user details)
    public static ClaimsPrincipal ValidateToken(string token, string securityKey, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(securityKey);

        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5147",
                ValidateAudience = true,
                ValidAudience = "http://localhost:5147",
                ValidateLifetime = validateLifetime, // Ensure that the token hasn't expired
                ClockSkew = TimeSpan.Zero // Default clock skew is 5 minutes; set to zero for immediate expiration
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

            return principal; // Returns the ClaimsPrincipal for the validated token
        }
        catch (SecurityTokenExpiredException)
        {
            throw new Exception("Token has expired.");
        }
        catch (Exception)
        {
            throw new Exception("Token validation failed.");
        }
    }
}
