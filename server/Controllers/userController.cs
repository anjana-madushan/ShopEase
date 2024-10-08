using System;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using server.DTOs;
using Microsoft.Extensions.Options;
using api.Dtos.Account;

namespace MongoExample.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly MongoDBService _mongoDBService;
        private readonly PasswordService _passwordService;
        private readonly JwtSettings _jwtSettings;
        private readonly EmailService _emailService;
        private readonly OTPService _otpService;

        // Constructor
        public UserController(MongoDBService mongoDBService, IOptions<JwtSettings> jwtSettings, PasswordService passwordService, OTPService otpService, EmailService emailService)
        {
            _mongoDBService = mongoDBService;
            _passwordService = passwordService;
            _otpService = otpService;
            _jwtSettings = jwtSettings.Value;
            _emailService = emailService;
        }

        // User Login with JWT Authentication
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (string.IsNullOrEmpty(login.Email))
                {
                    return BadRequest("Email cannot be null or empty.");
                }

                if (string.IsNullOrEmpty(login.Password))
                {
                    return BadRequest("Password cannot be null or empty.");
                }

                if (string.IsNullOrEmpty(login.Role))
                {
                    return BadRequest("Role cannot be null or empty.");
                }

                // Declare the variable for existing user
                dynamic existingUser = null;

                // Check if the role is "admin" and retrieve admin details
                if (login.Role.ToLower() == "admin")
                {
                    existingUser = await _mongoDBService.GetAdminByEmailAsync(login.Email);
                }
                else if (login.Role.ToLower() == "csr")
                {
                    existingUser = await _mongoDBService.GetCSRByEmailAsync(login.Email);
                }
                else if (login.Role.ToLower() == "vendor")
                {
                    existingUser = await _mongoDBService.GetVendorByEmailAsync(login.Email);
                }
                else if (login.Role.ToLower() == "customer")
                {
                    existingUser = await _mongoDBService.GetCustomerByEmailAsync(login.Email);
                }
                else
                {
                    return BadRequest("Invalid role provided.");
                }

                if (existingUser == null)
                {
                    return NotFound("A user with the provided email does not exist.");
                }

                Console.WriteLine("existingUser: " + existingUser.Password);

                // Validate the entered password with the stored hashed password
                bool isPasswordValid = _passwordService.VerifyPassword(login.Password, existingUser.Password);

                if (!isPasswordValid)
                {
                    return Unauthorized("The provided password is incorrect.");
                }

                var userResponse = new
                {
                    Id = existingUser.Id,
                    Username = existingUser.Username,
                    Email = existingUser.Email,
                    Role = login.Role.ToLower()
                };

                // Generate a JWT token and expire in 24 hours
                var token = JWTService.GenerateToken(existingUser, _jwtSettings.SecurityKey, 1440);

                return Ok(new { user = userResponse, token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }


        //Signup for new customer
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] NewUserDto signup)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the user already exists
                Users existingUser = await _mongoDBService.GetCustomerByEmailAsync(signup.Email);
                if (existingUser != null)
                {
                    return Conflict("A user with the provided email already exists.");
                }

                // Hash the password
                string hashedPassword = _passwordService.HashPassword(signup.Password);

                // Create a new user
                Users newUser = new Users
                {
                    Username = signup.UserName,
                    Email = signup.Email,
                    Password = hashedPassword,
                    ApprovalStatus = false
                };

                // Save the user to the database
                await _mongoDBService.CreateCustomerAsync(newUser);

                //Response
                var userResponse = new
                {
                    Id = newUser.Id,
                    Username = newUser.Username,
                    Email = newUser.Email,
                    Role = "customer"
                };

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        // Update user details
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserUpdateDTO updatedUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Validate token
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

                //Check if role is provided
                if (string.IsNullOrEmpty(updatedUser.Role))
                {
                    return BadRequest("Role is required.");
                }

                // Declare the variable for existing user
                dynamic existingUser = null;

                // Centralized user retrieval based on role
                switch (updatedUser.Role.ToLower())
                {
                    case "admin":
                        existingUser = await _mongoDBService.GetAdminByIdAsync(id);
                        break;
                    case "csr":
                        existingUser = await _mongoDBService.GetCSRByIdAsync(id);
                        break;
                    case "vendor":
                        existingUser = await _mongoDBService.GetVendorByIdAsync(id);
                        break;
                    case "customer":
                        existingUser = await _mongoDBService.GetCustomerByIdAsync(id);
                        break;
                    default:
                        return BadRequest("Invalid role provided.");
                }

                if (existingUser == null)
                {
                    return NotFound("A user with the provided id does not exist.");
                }

                if (!string.IsNullOrEmpty(updatedUser.Username))
                {
                    existingUser.Username = updatedUser.Username;
                }
                else
                {
                    existingUser.Username = existingUser.Username;
                }


                if (!string.IsNullOrEmpty(updatedUser.Email))
                {
                    existingUser.Email = updatedUser.Email;
                }
                else
                {
                    existingUser.Email = existingUser.Email;
                }

                if (!string.IsNullOrEmpty(updatedUser.Password))
                {
                    existingUser.Password = _passwordService.HashPassword(updatedUser.Password);
                }
                else
                {
                    existingUser.Password = existingUser.Password;
                }


                if (string.IsNullOrEmpty(updatedUser.Username) && string.IsNullOrEmpty(updatedUser.Email) && string.IsNullOrEmpty(updatedUser.Password) && string.IsNullOrEmpty(updatedUser.Role))
                {
                    return BadRequest("Please provide the details to update.");
                }

                //Response
                var userResponse = new
                {
                    Id = existingUser.Id,
                    Username = existingUser.Username,
                    Email = existingUser.Email,
                    Role = updatedUser.Role.ToLower()
                };

                await _mongoDBService.UpdateUserAsync(id, updatedUser.Role.ToLower(), existingUser);

                return Ok(userResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        // Get all user details based on role
        [HttpGet("all/{role}")]
        public async Task<IActionResult> GetAllUsers(string role)
        {
            try
            {        //Validate token
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
                // Declare the variable for existing user
                dynamic existingUsers = null;

                // Centralized user retrieval based on role
                switch (role.ToLower())
                {
                    case "admin":
                        existingUsers = await _mongoDBService.GetAllAdminsAsync();
                        break;
                    case "csr":
                        existingUsers = await _mongoDBService.GetAllCSRsAsync();
                        break;
                    case "vendor":
                        existingUsers = await _mongoDBService.GetVendorsAsync();
                        break;
                    case "customer":
                        existingUsers = await _mongoDBService.GetAllCustomersAsync();
                        break;
                    default:
                        return BadRequest("Invalid role provided.");
                }

                if (existingUsers == null)
                {
                    return NotFound("No users found.");
                }


                return Ok(existingUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        // Get user details by ID based on role
        [HttpGet("{role}/{id}")]
        public async Task<IActionResult> GetUserById(string id, string role)
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
                // Declare the variable for existing user
                dynamic existingUser = null;

                // Centralized user retrieval based on role
                switch (role.ToLower())
                {
                    case "admin":
                        existingUser = await _mongoDBService.GetAdminByIdAsync(id);
                        break;
                    case "csr":
                        existingUser = await _mongoDBService.GetCSRByIdAsync(id);
                        break;
                    case "vendor":
                        existingUser = await _mongoDBService.GetVendorByIdAsync(id);
                        break;
                    case "customer":
                        existingUser = await _mongoDBService.GetCustomerByIdAsync(id);
                        break;
                    default:
                        return BadRequest("Invalid role provided.");
                }

                if (existingUser == null)
                {
                    return NotFound("A user with the provided ID does not exist.");
                }

                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Get user details by email based on role
        [HttpGet("{role}/email/{email}")]
        public async Task<IActionResult> GetUserByEmail(string email, string role)
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
                // Declare the variable for existing user
                dynamic existingUser = null;

                // Centralized user retrieval based on role
                switch (role.ToLower())
                {
                    case "admin":
                        existingUser = await _mongoDBService.GetAdminByEmailAsync(email);
                        break;
                    case "csr":
                        existingUser = await _mongoDBService.GetCSRByEmailAsync(email);
                        break;
                    case "vendor":
                        existingUser = await _mongoDBService.GetVendorByEmailAsync(email);
                        break;
                    case "customer":
                        existingUser = await _mongoDBService.GetCustomerByEmailAsync(email);
                        break;
                    default:
                        return BadRequest("Invalid role provided.");
                }

                if (existingUser == null)
                {
                    return NotFound("A user with the provided email does not exist.");
                }

                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Approve Customer
        [HttpPut("approve/{id}/{email}")]
        public async Task<IActionResult> ApproveCustomer(string id, string email)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Validate for parameters
                if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(email))
                {
                    return BadRequest("Please provide the customer ID and email.");
                }
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

                bool isAuthorized = false;
                bool isAdmin = false;
                bool isCSR = false;
                dynamic existingAuthenticator = null;

                // Check if the user is an admin or csr
                existingAuthenticator = await _mongoDBService.GetAdminByEmailAsync(email);

                if (existingAuthenticator != null)
                {
                    isAuthorized = true;
                    isAdmin = true;
                }
                else
                {
                    existingAuthenticator = await _mongoDBService.GetCSRByEmailAsync(email);
                    if (existingAuthenticator != null)
                    {
                        isAuthorized = true;
                        isCSR = true;
                    }
                }
                if (!isAuthorized)
                {
                    return Unauthorized("You are not authorized to approve customers.");
                }

                // Get the customer by ID
                Users existingUser = await _mongoDBService.GetCustomerByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound("A customer with the provided ID does not exist.");
                }

                // Check if the customer is already approved
                if (existingUser.ApprovalStatus)
                {
                    return Conflict("The customer is already approved.");
                }

                // Update the customer approval status
                existingUser.ApprovalStatus = true;
                existingUser.ApprovedBy = id;

                // Save the updated user details 
                await _mongoDBService.UpdateCustomer(id, existingUser);

                // Send an email to the customer
                await _emailService.SendEmailAsync(existingUser.Email, "Account Approval", "Your account has been approved.");
                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        // Get all approved customers
        [HttpGet("approved/customers")]
        public async Task<IActionResult> GetApprovedCustomers()
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

                // Get all approved customers
                var approvedCustomers = await _mongoDBService.GetApprovedCustomersAsync();

                return Ok(approvedCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Get all unapproved customers
        [HttpGet("unapproved/customers")]
        public async Task<IActionResult> GetUnapprovedCustomers()
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

                // Get all unapproved customers
                var unapprovedCustomers = await _mongoDBService.GetUnapprovedCustomersAsync();

                return Ok(unapprovedCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Get all approved Customers by ID
        [HttpGet("approved/customers/{id}")]
        public async Task<IActionResult> GetApprovedCustomersById(string id)
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

                // Get all approved customers by ID
                var approvedCustomers = await _mongoDBService.GetApprovedCustomersByIdAsync(id);

                return Ok(approvedCustomers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Deactivate Customer
        [HttpPut("deactivate/{id}/{email}/{role}")]
        public async Task<IActionResult> DeactivateCustomer(string id, string email, string role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Validate for parameters
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Please provide the customer ID.");
                }
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

                //Validate CSR or Admin
                dynamic existingAuthenticator = null;
                if (role.ToLower() == "admin")
                {
                    existingAuthenticator = await _mongoDBService.GetAdminByEmailAsync(email);
                }
                else if (role.ToLower() == "csr")
                {
                    existingAuthenticator = await _mongoDBService.GetCSRByEmailAsync(email);
                }

                if (existingAuthenticator == null)
                {
                    return Unauthorized("You are not authorized to deactivate customers.");
                }

                // Get the customer by ID
                Users existingUser = await _mongoDBService.GetCustomerByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound("A customer with the provided ID does not exist.");
                }

                // Check if the customer is already deactivated
                if (!existingUser.ApprovalStatus)
                {
                    return Conflict("The customer is already deactivated.");
                }

                // Update the customer approval status
                existingUser.Deactivated = true;
                existingUser.DeactivatedBy = existingAuthenticator.Id;

                // Save the updated user details 
                await _mongoDBService.UpdateCustomer(id, existingUser);

                return Ok(existingUser);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Reactivate Customer
        [HttpPut("reactivate/{id}/{email}/{role}")]
        public async Task<IActionResult> ReactivateCustomer(string id, string email, string role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                //Validate for parameters
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest("Please provide the customer ID.");
                }
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

                //Validate CSR or Admin
                dynamic existingAuthenticator = null;
                if (role.ToLower() == "admin")
                {
                    existingAuthenticator = await _mongoDBService.GetAdminByEmailAsync(email);
                }
                else if (role.ToLower() == "csr")
                {
                    existingAuthenticator = await _mongoDBService.GetCSRByEmailAsync(email);
                }

                if (existingAuthenticator == null)
                {
                    return Unauthorized("You are not authorized to deactivate customers.");
                }

                // Get the customer by ID
                Users existingUser = await _mongoDBService.GetCustomerByIdAsync(id);
                if (existingUser == null)
                {
                    return NotFound("A customer with the provided ID does not exist.");
                }

                // Check if the customer is already deactivated
                if (!existingUser.ApprovalStatus)
                {
                    return Conflict("The customer is already deactivated.");
                }

                // Update the customer approval status
                existingUser.Deactivated = false;
                existingUser.ReactivatedBy = existingAuthenticator.Id;

                // Save the updated user details 
                await _mongoDBService.UpdateCustomer(id, existingUser);

                return Ok(existingUser);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        // Get all deactivated customers
        [HttpGet("deactivated/customers")]
        public async Task<IActionResult> GetDeactivatedCustomers()
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

                // Get all deactivated customers
                var deactivatedCustomers = await _mongoDBService.GetDeactivatedCustomersAsync();

                return Ok(deactivatedCustomers);

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Generate OTP for password reset
        [HttpPost("generateOTP/{email}/{role}")]
        public async Task<IActionResult> GenerateOTP(string email, string role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the user exists
                dynamic existingUser = null;

                // Centralized user retrieval based on role
                switch (role.ToLower())
                {
                    case "admin":
                        existingUser = await _mongoDBService.GetAdminByEmailAsync(email);
                        break;
                    case "csr":
                        existingUser = await _mongoDBService.GetCSRByEmailAsync(email);
                        break;
                    case "vendor":
                        existingUser = await _mongoDBService.GetVendorByEmailAsync(email);
                        break;
                    case "customer":
                        existingUser = await _mongoDBService.GetCustomerByEmailAsync(email);
                        break;
                    default:
                        return BadRequest("Invalid role provided.");
                }

                if (existingUser == null)
                {
                    return NotFound("A user with the provided email does not exist.");
                }

                // Send OTP to the user
                await _otpService.SendOTPAsync(email);

                return Ok("An OTP has been sent to your email.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Validate OTP for password reset
        [HttpPost("validateOTP/{email}/{role}")]
        public async Task<IActionResult> ValidateOTP(string email, string role, [FromBody] OTPDto otp)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the user exists
                dynamic existingUser = null;

                //Validate OTP
                if (string.IsNullOrEmpty(otp.Code))
                {
                    return BadRequest("Please provide the OTP code.");
                }

                // Centralized user retrieval based on role
                switch (role.ToLower())
                {
                    case "admin":
                        existingUser = await _mongoDBService.GetAdminByEmailAsync(email);
                        break;
                    case "csr":
                        existingUser = await _mongoDBService.GetCSRByEmailAsync(email);
                        break;
                    case "vendor":
                        existingUser = await _mongoDBService.GetVendorByEmailAsync(email);
                        break;
                    case "customer":
                        existingUser = await _mongoDBService.GetCustomerByEmailAsync(email);
                        break;
                    default:
                        return BadRequest("Invalid role provided.");
                }

                if (existingUser == null)
                {
                    return NotFound("A user with the provided email does not exist.");
                }

                // Validate the OTP
                bool isOTPValid = _otpService.ValidateOTP(email, otp.Code);
                if (!isOTPValid)
                {
                    return Unauthorized("The provided OTP is incorrect.");
                }

                return Ok("The OTP is valid.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }
        }

        //Reset Password
        [HttpPut("resetPassword/{email}/{role}")]
        public async Task<IActionResult> ResetPassword(string email, string role, [FromBody] ResetPasswordDto resetPassword)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check if the user exists
                dynamic existingUser = null;

                //Validate password
                if (string.IsNullOrEmpty(resetPassword.Password))
                {
                    return BadRequest("Please provide the new password.");
                }
                //Validate OTP
                if (string.IsNullOrEmpty(resetPassword.Code))
                {
                    return BadRequest("Please provide the OTP code.");
                }

                // Validate the OTP
                bool isOTPValid = _otpService.ValidateOTPOnReset(email, resetPassword.Code);
                if (!isOTPValid)
                {
                    return Unauthorized("The provided OTP is incorrect.");
                }


                // Centralized user retrieval based on role
                switch (role.ToLower())
                {
                    case "admin":
                        existingUser = await _mongoDBService.GetAdminByEmailAsync(email);
                        break;
                    case "csr":
                        existingUser = await _mongoDBService.GetCSRByEmailAsync(email);
                        break;
                    case "vendor":
                        existingUser = await _mongoDBService.GetVendorByEmailAsync(email);
                        break;
                    case "customer":
                        existingUser = await _mongoDBService.GetCustomerByEmailAsync(email);
                        break;
                    default:
                        return BadRequest("Invalid role provided.");
                }

                if (existingUser == null)
                {
                    return NotFound("A user with the provided email does not exist.");
                }

                // Hash the new password
                string hashedPassword = _passwordService.HashPassword(resetPassword.Password);

                // Update the user password
                existingUser.Password = hashedPassword;

                var userId = existingUser.Id;

                // Save the updated user details
                await _mongoDBService.UpdateUserPasswordAsync(userId, hashedPassword, role);

                return Ok("Password reset successful.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An internal server error occurred: {ex.Message}");
            }

        }


    }
}


