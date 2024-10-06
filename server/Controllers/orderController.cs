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
                Status = server.Models.OrderStatus.Processing,
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

        //Request to cancel an order by customer after Dispatched
        [HttpPut("request-to-cancel-order/{orderId}")]
        public async Task<IActionResult> RequestToCancelOrder(string orderId, CancelOrderRequestDTO cancelOrderRequestDTO)
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
            if (orderId == null || cancelOrderRequestDTO.Note == null)
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
            if (order.Status != server.Models.OrderStatus.Dispatched)
            {
                return BadRequest("Order not dispatched. Cannot request to cancel.");
            }

            // Update the order status to RequestToCancel
            order.RequestToCancel = true;
            order.StatusUpdatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            order.Note = cancelOrderRequestDTO.Note;

            //Add to Notification
            var notification = await _mongoDBService.CreateNotification(new Notification
            {
                Message = "Request to cancel order",
                Date = DateTime.Now,
                Read = false,
                UserId = order.UserId
            });

            if (notification == null)
            {
                return BadRequest("Failed to add notification.");
            }

            //Send email notification
            var userDetail = await _mongoDBService.GetCustomerByIdAsync(order.UserId);

            if (userDetail != null)
            {
                await _emailService.SendEmailAsync(userDetail.Email, "Request to cancel order", "Your request to cancel the order has been received.");
            }

            await _mongoDBService.UpdateOrder(order);
            return Ok("Request to cancel order sent successfully.");
        }

        //Get all requests to cancel orders
        [HttpGet("requests-to-cancel-order")]
        public async Task<IActionResult> GetAllRequestsToCancelOrders()
        {
            try
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

                var orders = await _mongoDBService.GetRequestToCancelOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Get all orders where cancelled is true but cancel request is false
        [HttpGet("cancelled-orders")]
        public async Task<IActionResult> GetAllCancelledOrders()
        {
            try
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

                var orders = await _mongoDBService.GetCancelledOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        //Update the status of order to delivered by admin || vendor || csr
        [HttpPut("order-status-delivered/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatusDelivered(string orderId, StatusUpdateDTO statusUpdateDTO)
        {
            try
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

                // Check if the order is already delivered
                if (order.Status == server.Models.OrderStatus.Delivered)
                {
                    return BadRequest("Order already delivered.");
                }

                dynamic userDetail = null;
                //Verify the user role
                if (statusUpdateDTO.Role == "admin")
                {
                    userDetail = await _mongoDBService.GetAdminByIdAsync(statusUpdateDTO.userId);
                }
                else if (statusUpdateDTO.Role == "vendor")
                {
                    userDetail = await _mongoDBService.GetVendorByIdAsync(statusUpdateDTO.userId);
                }
                else if (statusUpdateDTO.Role == "csr")
                {
                    userDetail = await _mongoDBService.GetCSRByIdAsync(statusUpdateDTO.userId);
                }
                else
                {
                    return BadRequest("Invalid role.");
                }

                // Verify the user
                if (userDetail == null || userDetail.Id != statusUpdateDTO.userId)
                {
                    return NotFound("User not found or user ID does not match.");
                }

                //Get Customer details
                var customerDetail = await _mongoDBService.GetCustomerByIdAsync(order.UserId);

                if (customerDetail == null)
                {
                    return BadRequest("Customer not found.");
                }

                //Check if request to cancel is true
                if (order.RequestToCancel == true)
                {
                    return BadRequest("Order has been requested to cancel. Cannot deliver.");
                }

                // Update the order status to Delivered
                order.Status = server.Models.OrderStatus.Delivered;
                order.StatusUpdatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                //Add to Notification
                var notification = await _mongoDBService.CreateNotification(new Notification
                {
                    Message = "Order delivered",
                    Date = DateTime.Now,
                    Read = false,
                    UserId = order.UserId
                });

                //Send email notification
                await _emailService.SendEmailAsync(customerDetail.Email, "Request to cancel order", "Your request to cancel the order has been received.");

                await _mongoDBService.UpdateOrder(order);
                return Ok("Order status updated to delivered successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Update Order to Dispatched by admin || vendor || csr
        [HttpPut("order-status-dispatched/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatusDispatched(string orderId, StatusUpdateDTO statusUpdateDTO)
        {
            try
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
                if (orderId == null || statusUpdateDTO.Role == null || statusUpdateDTO.userId == null)
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
                    return BadRequest("Order already dispatched.");
                }

                dynamic userDetail = null;
                //Verify the user role
                if (statusUpdateDTO.Role == "admin")
                {
                    userDetail = await _mongoDBService.GetAdminByIdAsync(statusUpdateDTO.userId);
                }
                else if (statusUpdateDTO.Role == "vendor")
                {
                    userDetail = await _mongoDBService.GetVendorByIdAsync(statusUpdateDTO.userId);
                }
                else if (statusUpdateDTO.Role == "csr")
                {
                    userDetail = await _mongoDBService.GetCSRByIdAsync(statusUpdateDTO.userId);
                }
                else
                {
                    return BadRequest("Invalid role.");
                }

                // Verify the user
                if (userDetail == null || userDetail.Id != statusUpdateDTO.userId)
                {
                    return NotFound("User not found or user ID does not match.");
                }

                // Update the order status to Dispatched
                order.Status = server.Models.OrderStatus.Dispatched;
                order.StatusUpdatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                //Add to Notification
                var notification = await _mongoDBService.CreateNotification(new Notification
                {
                    Message = "Order dispatched",
                    Date = DateTime.Now,
                    Read = false,
                    UserId = order.UserId
                });

                //Send email notification
                var customerDetail = await _mongoDBService.GetCustomerByIdAsync(order.UserId);

                await _mongoDBService.UpdateOrder(order);
                return Ok("Order status updated to dispatched successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Update  Order status to Ready by vendor 
        [HttpPut("order-status-ready/{orderId}")]
        public async Task<IActionResult> UpdateOrderStatusReady(string orderId, StatusUpdateDTO statusUpdateDTO)
        {
            try
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
                if (orderId == null || statusUpdateDTO.Role == null || statusUpdateDTO.userId == null)
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

                // Check if the order is already ready
                if (order.Status == server.Models.OrderStatus.Ready)
                {
                    return BadRequest("Order already ready.");
                }

                dynamic userDetail = null;
                //Verify the user role
                if (statusUpdateDTO.Role == "vendor")
                {
                    userDetail = await _mongoDBService.GetVendorByIdAsync(statusUpdateDTO.userId);
                }
                else
                {
                    return BadRequest("Invalid role.");
                }

                // Verify the user
                if (userDetail == null || userDetail.Id != statusUpdateDTO.userId)
                {
                    return NotFound("User not found or user ID does not match.");
                }

                // Update the order status to Ready
                order.Status = server.Models.OrderStatus.Ready;
                order.StatusUpdatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");

                //Add to Notification
                var notification = await _mongoDBService.CreateNotification(new Notification
                {
                    Message = "Order ready",
                    Date = DateTime.Now,
                    Read = false,
                    UserId = order.UserId
                });

                //Send email notification
                var customerDetail = await _mongoDBService.GetCustomerByIdAsync(order.UserId);

                await _mongoDBService.UpdateOrder(order);
                return Ok("Order status updated to ready successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        //Approve a request to cancel an order by  csr
        [HttpPut("approve-request-to-cancel-order/{orderId}")]
        public async Task<IActionResult> ApproveRequestToCancelOrder(string orderId, StatusUpdateDTO statusUpdateDTO)
        {
            try
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
                if (orderId == null || statusUpdateDTO.Role == null || statusUpdateDTO.userId == null)
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

                // Check if the order is already cancelled
                if (order.Status == server.Models.OrderStatus.Cancelled)
                {
                    return BadRequest("Order already cancelled.");
                }

                dynamic userDetail = null;
                //Verify the user role
                if (statusUpdateDTO.Role == "csr")
                {
                    userDetail = await _mongoDBService.GetCSRByIdAsync(statusUpdateDTO.userId);
                }
                else
                {
                    return BadRequest("Invalid role.");
                }

                // Verify the user
                if (userDetail == null || userDetail.Id != statusUpdateDTO.userId)
                {
                    return NotFound("User not found or user ID does not match.");
                }

                // Update the order status to Cancelled
                order.Status = server.Models.OrderStatus.Cancelled;
                order.StatusUpdatedOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                order.CancelledOn = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
                order.CancelledBy = statusUpdateDTO.userId;
                order.Cancelled = true;
                order.RequestToCancel = false;

                //Add to Notification
                var notification = await _mongoDBService.CreateNotification(new Notification
                {
                    Message = "Order cancelled",
                    Date = DateTime.Now,
                    Read = false,
                    UserId = order.UserId
                });

                //Send email notification
                var customerDetail = await _mongoDBService.GetCustomerByIdAsync(order.UserId);

                await _mongoDBService.UpdateOrder(order);
                return Ok("Order status updated to cancelled successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
