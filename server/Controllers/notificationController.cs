using System;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using server.DTOs;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MongoExample.Controllers;

[Route("api/notification")]
[ApiController]
public class NotificationController : ControllerBase
{

  private readonly MongoDBService _mongoDBService;
  private readonly JwtSettings _jwtSettings;

  public NotificationController(MongoDBService mongoDBService, IOptions<JwtSettings> jwtSettings, PasswordService passwordService)
  {
    _mongoDBService = mongoDBService;
    _jwtSettings = jwtSettings.Value;
  }

  //Delete a notification
  [HttpDelete("delete-notification")]
  public async Task<IActionResult> DeleteNotification(string notificationId)
  {
    //validate token
    var token = Request.Headers["Authorization"];
    if (token.Count == 0)
    {
      return Unauthorized("Token is required.");
    }

    var user = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

    if (user == null)
    {
      return Unauthorized("Invalid token.");
    }

    //Validate necessary fields
    if (notificationId == null)
    {
      return BadRequest("Missing required fields.");
    }

    // Retrieve notification details
    var notification = await _mongoDBService.GetNotificationByIdAsync(notificationId);

    // Verify the notification
    if (notification == null || notification.Id != notificationId)
    {
      return NotFound("Notification not found or notification ID does not match.");
    }

    // Delete the notification
    await _mongoDBService.DeleteNotificationAsync(notificationId);

    return Ok("Notification deleted successfully.");

  }

  [HttpGet]
  public async Task<IActionResult> GetNotifications()
  {
    //validate token
    var token = Request.Headers["Authorization"];
    if (token.Count == 0)
    {
      return Unauthorized("Token is required.");
    }

    var user = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

    if (user == null)
    {
      return Unauthorized("Invalid token.");
    }

    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userId == null)
    {
      return Unauthorized("User ID not found in token.");
    }

    var notifications = await _mongoDBService.GetNotificationsByUserID(userId);

    if (notifications == null || notifications.Count == 0)
    {
      return NotFound("No notifications found for the user.");
    }

    // Return the notifications
    return Ok(notifications);

  }
}
