using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using server.DTOs;
using Microsoft.Extensions.Options;
using YourNamespace.Services;

namespace MongoExample.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly JwtSettings _jwtSettings;
        private readonly EmailService _emailService;

        public OrderController(MongoDBService mongoDBService, IOptions<JwtSettings> jwtSettings, EmailService emailService)
        {
            _mongoDBService = mongoDBService;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
        }

        // Create a new Order from the orderDTO
        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder(OrderDTO orderDTO)
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
            if (orderDTO.ShippingAddress == null || orderDTO.BillingAddress == null || orderDTO.Email == null || orderDTO.Products == null || orderDTO.TotalPrice == 0 || orderDTO.TotalQty == 0 || orderDTO.UserId == null)
            {
                return BadRequest("Missing required fields.");
            }

            // Retrieve user details
            var userDetail = await _mongoDBService.GetCustomerByIdAsync(orderDTO.UserId);

            // Verify the user
            if (userDetail == null || userDetail.Id != orderDTO.UserId)
            {
                return NotFound("User not found or user ID does not match.");
            }

            //Check if the number of products is greater than 0
            if (orderDTO.Products.Count < 1)
            {
                return BadRequest("At least one product is required.");
            }

            //Check if the product exists
            foreach (var product in orderDTO.Products)
            {
                var productDetail = await _mongoDBService.GetProductAsync(product.Key);
                if (productDetail == null || productDetail.Id != product.Key)
                {
                    return NotFound("Product not found or product ID does not match.");
                }
            }

            //Create a random unique 4 digit order ID not in the database
            Random random = new Random();
            string orderID = random.Next(1000, 9999).ToString();
            while (await _mongoDBService.GetOrderByOrderIdAsync(orderID) != null)
            {
                orderID = random.Next(1000, 9999).ToString();
            }

            //Check if email already exists
            var orderEmail = await _mongoDBService.GetCustomerByEmailAsync(orderDTO.Email);

            if (orderEmail == null)
            {
                return BadRequest("Email does not exist.");
            }

            //Add to Notification
            var notification = await _mongoDBService.CreateNotification(new Notification
            {
                Message = "Order created",
                Date = DateTime.Now,
                Read = false,
                UserId = orderDTO.UserId
            });

            //Send email notification
            if (userDetail != null)
            {
                await _emailService.SendEmailAsync(userDetail.Email, "Order created", "Your order has been created.");
            }

            // Create a new Order
            var order = new Order
            {
                OrderId = orderID,
                OrderDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                Status = server.Models.OrderStatus.Ordered,
                StatusUpdatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss"),
                ShippingAddress = orderDTO.ShippingAddress,
                BillingAddress = orderDTO.BillingAddress,
                Email = orderDTO.Email,
                Products = orderDTO.Products.ToDictionary(
                    x => x.Key,
                    x => new ProductDetails { Price = x.Value.Price, Quantity = x.Value.Quantity }
                ),
                TotalPrice = orderDTO.TotalPrice,
                TotalQty = orderDTO.TotalQty,
                PaymentStatus = true,
                UserId = orderDTO.UserId
            };

            await _mongoDBService.CreateOrder(order);
            return Ok(order);
        }

        //Cancel an order by customer before Dispatched
        [HttpPut("cancel-order/{orderId}")]
        public async Task<IActionResult> CancelOrder(string orderId)
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
            if (orderId == null)
            {
                return BadRequest("Missing required fields.");
            }

            // Retrieve order details
            var order = await _mongoDBService.GetOrderByOrderIdAsync(orderId);

            // Verify the order
            if (order == null || order.OrderId != orderId)
            {
                return NotFound("Order not found or order ID does not match.");
            }

            // Check if the order is already dispatched
            if (order.Status == server.Models.OrderStatus.Dispatched)
            {
                return BadRequest("Order already dispatched. Cannot cancel.");
            }

            // Update the order status to Cancelled
            order.Status = server.Models.OrderStatus.Cancelled;
            order.StatusUpdatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            order.CancelledOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            order.CancelledBy = order.UserId;
            order.Cancelled = true;
            
            //Add to Notification
            var notification = await _mongoDBService.CreateNotification(new Notification
            {
                Message = "Order cancelled",
                Date = DateTime.Now,
                Read = false,
                UserId = order.UserId
            });

            //Send email notification
            var userDetail = await _mongoDBService.GetCustomerByIdAsync(order.UserId);

            await _mongoDBService.UpdateOrder(order);
            return Ok("Order cancelled successfully.");
        }
    }
}
