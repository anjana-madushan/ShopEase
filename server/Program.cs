using server.Models;
using server.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS configuration
builder.Services.AddCors(options =>
{
  options.AddPolicy("AllowAll",
      builder => builder
          .AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());
});

// Configure MongoDB service
builder.Services.Configure<MongoDBConfig>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDBService>();

// Register PasswordService
builder.Services.AddSingleton<PasswordService>();

// Register the memory cache service 
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<EmailService>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    var apiKey = config["SendGrid:ApiKey"];
    var senderEmail = config["SendGrid:SenderEmail"];
    var senderName = config["SendGrid:SenderName"];
    return new EmailService(apiKey, senderEmail, senderName);
});

builder.Services.AddSingleton<OTPService>();


// Configure JWT settings
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettingsSection);

var jwtSettings = jwtSettingsSection.Get<JwtSettings>();
var key = Encoding.ASCII.GetBytes(jwtSettings.SecurityKey);

// Configure authentication with JWT
builder.Services.AddAuthentication(options =>
{
  options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
  options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
  options.RequireHttpsMetadata = false;
  options.SaveToken = true;
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(key),
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidIssuer = jwtSettings.Issuer,
    ValidAudience = jwtSettings.Audience
  };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseRouting();

// Enable Authentication
app.UseAuthentication(); // Add this before authorization
app.UseAuthorization();

app.MapControllers();

// Error handling middleware
app.Use(async (context, next) =>
{
  try
  {
    await next();
  }
  catch (Exception ex)
  {
    Console.WriteLine($"An error occurred: {ex.Message}");
    context.Response.StatusCode = 500;
    await context.Response.WriteAsync("An unexpected error occurred.");
  }
});

app.Run();
