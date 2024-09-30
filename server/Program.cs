using server.Models;
using server.Services;

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

builder.Services.Configure<MongoDBConfig>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<MongoDBService>();

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

// app.UseAuthorization();

app.MapControllers();

//Add error handling middleware
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