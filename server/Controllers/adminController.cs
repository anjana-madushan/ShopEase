using System;
using Microsoft.AspNetCore.Mvc;
using server.Services;
using server.Models;
using server.DTOs;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace MongoExample.Controllers;

[Route("api/admin")]
[ApiController]
public class AdminController : ControllerBase
{

    private readonly MongoDBService _mongoDBService;
    private readonly PasswordService _passwordService;
    private readonly JwtSettings _jwtSettings;

    public AdminController(MongoDBService mongoDBService, IOptions<JwtSettings> jwtSettings, PasswordService passwordService)
    {
        _mongoDBService = mongoDBService;
        _jwtSettings = jwtSettings.Value;
        _passwordService = passwordService;
    }

    //Create a new Admin user call the DTO
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] AdminDTO admin)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        //Check if the provided email is valid
        if (!admin.Email.Contains("@"))
        {
            return BadRequest("The provided email is not valid.");
        }
        //Check if the provided password is valid
        if (admin.Password.Length < 8 || admin.Password.Length > 20 ||
      !admin.Password.Any(char.IsDigit) ||
      !admin.Password.Any(char.IsUpper) ||
      !admin.Password.Any(char.IsLower) ||
      !admin.Password.Any(c => !char.IsLetterOrDigit(c)))
        {
            return BadRequest("The provided password is not valid (must be between 8 and 20 characters and contain at least one uppercase letter, one lowercase letter, one digit, and one special character).");
        }
        // Check if an admin with the provided email already exists
        var existingAdmin = await _mongoDBService.GetAdminByEmailAsync(admin.Email);
        if (existingAdmin != null)
        {
            return Conflict("An admin with the provided email already exists.");
        }

        // Hash the password
        string hashedPassword = _passwordService.HashPassword(admin.Password);
        var newAdmin = new Admin
        {
            Username = admin.Username,
            Password = hashedPassword,
            Email = admin.Email
        };

        await _mongoDBService.CreateAdminAsync(newAdmin);
        return CreatedAtAction(nameof(Get), new { id = newAdmin.Id }, newAdmin);
    }

    //Get Admin by ID
    [HttpGet("{id}")]
    public async Task<ActionResult<Admin>> Get(string id)
    {
        var admin = await _mongoDBService.GetAdminByIdAsync(id);

        if (admin is null)
        {
            return NotFound();
        }

        return admin;
    }

    //Admin creating another Admin
    [HttpPost("create/admin")]
    public async Task<IActionResult> CreateAdmin([FromBody] AdminDTO admin)
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
                return Unauthorized("No token provided.");
            }

            // Call JWT Service to validate the token
            var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

            // Check if the token is valid
            if (tokenverify == null)
            {
                return Unauthorized("Invalid token.");
            }



            // Check if the email is in the token
            if (tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) == null)
            {
                return Unauthorized("Invalid token.");
            }

            //Check if that user is avalable in the Admin table
            var adminUser = await _mongoDBService.GetAdminByEmailAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);

            //Check if the user is an admin
            if (adminUser == null)
            {
                return Unauthorized("Unauthorized user.");
            }


            //Check if the admin is exist
            var adminList = await _mongoDBService.GetAdminByIdAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);


            //Check if the provided email is valid
            if (!admin.Email.Contains("@"))
            {
                return BadRequest("The provided email is not valid.");
            }
            //Check if the provided password is valid
            if (admin.Password.Length < 8 || admin.Password.Length > 20 ||
          !admin.Password.Any(char.IsDigit) ||
          !admin.Password.Any(char.IsUpper) ||
          !admin.Password.Any(char.IsLower) ||
          !admin.Password.Any(c => !char.IsLetterOrDigit(c)))
            {
                return BadRequest("The provided password is not valid (must be between 8 and 20 characters and contain at least one uppercase letter, one lowercase letter, one digit, and one special character).");
            }
            // Check if an admin with the provided email already exists
            var existingAdmin = await _mongoDBService.GetAdminByEmailAsync(admin.Email);
            if (existingAdmin != null)
            {
                return Conflict("An admin with the provided email already exists.");
            }

            // Hash the password
            string hashedPassword = _passwordService.HashPassword(admin.Password);
            var newAdmin = new Admin
            {
                Username = admin.Username,
                Password = hashedPassword,
                Email = admin.Email
            };

            await _mongoDBService.CreateAdminAsync(newAdmin);

            //Find the admin by id and update the admin List
            adminList.AdminsCreated.Add(newAdmin);
            await _mongoDBService.UpdateAdminAsync(adminList.Id, adminList);


            return CreatedAtAction(nameof(Get), new { id = newAdmin.Id }, newAdmin);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal server error occurred: {ex.Message}");
        }
    }


    //Admin creating a CSR
    [HttpPost("create/csr")]
    public async Task<IActionResult> CreateCSR([FromBody] CSRDTO csr)
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
                return Unauthorized("No token provided.");
            }

            // Call JWT Service to validate the token
            var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

            // Check if the token is valid
            if (tokenverify == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Check if the email is in the token
            if (tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) == null)
            {
                return Unauthorized("Invalid token.");
            }

            //Check if that user is avalable in the Admin table
            var adminUser = await _mongoDBService.GetAdminByEmailAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);

            //Check if the user is an admin
            if (adminUser == null)
            {
                return Unauthorized("Unauthorized user.");
            }

            //Check if the admin is exist
            var adminList = await _mongoDBService.GetAdminByIdAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            //Check if the provided email is valid
            if (!csr.Email.Contains("@"))
            {
                return BadRequest("The provided email is not valid.");
            }

            // Check if an admin with the provided email already exists
            var existingCSR = await _mongoDBService.GetCSRByEmailAsync(csr.Email);
            if (existingCSR != null)
            {
                return Conflict("A CSR with the provided email already exists.");
            }

            // Hash the password
            string hashedPassword = _passwordService.HashPassword(csr.Password);
            var newCSR = new CSR
            {
                Username = csr.Username,
                Password = hashedPassword,
                Email = csr.Email
            };

            await _mongoDBService.CreateCSRAsync(newCSR);

            //Find the admin by id and update the admin List
            adminList.CSRCreated.Add(newCSR);
            await _mongoDBService.UpdateAdminAsync(adminList.Id, adminList);

            return CreatedAtAction(nameof(Get), new { id = newCSR.Id }, newCSR);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal server error occurred: {ex.Message}");
        }
    }

    //Admin create a Vendor
    [HttpPost("create/vendor")]
    public async Task<IActionResult> CreateVendor([FromBody] VendorDTO vendor)
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
                return Unauthorized("No token provided.");
            }

            // Call JWT Service to validate the token
            var tokenverify = JWTService.ValidateToken(token, _jwtSettings.SecurityKey);

            // Check if the token is valid
            if (tokenverify == null)
            {
                return Unauthorized("Invalid token.");
            }

            // Check if the email is in the token
            if (tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email) == null)
            {
                return Unauthorized("Invalid token.");
            }

            //Check if that user is avalable in the Admin table
            var adminUser = await _mongoDBService.GetAdminByEmailAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value);

            //Check if the user is an admin
            if (adminUser == null)
            {
                return Unauthorized("Unauthorized user.");
            }

            //Check if the admin is exist
            var adminList = await _mongoDBService.GetAdminByIdAsync(tokenverify.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);

            //Check if the provided email is valid
            if (!vendor.Email.Contains("@"))
            {
                return BadRequest("The provided email is not valid.");
            }

            // Check if an admin with the provided email already exists
            var existingVendor = await _mongoDBService.GetVendorByEmailAsync(vendor.Email);
            if (existingVendor != null)
            {
                return Conflict("A Vendor with the provided email already exists.");
            }

            // Hash the password
            string hashedPassword = _passwordService.HashPassword(vendor.Password);
            var newVendor = new Vendor
            {
                Username = vendor.Username,
                Password = hashedPassword,
                Email = vendor.Email
            };

            await _mongoDBService.CreateVendorAsync(newVendor);

            //Find the admin by id and update the admin List
            adminList.VendorCreated.Add(newVendor);
            await _mongoDBService.UpdateAdminAsync(adminList.Id, adminList);

            return CreatedAtAction(nameof(Get), new { id = newVendor.Id }, newVendor);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An internal server error occurred: {ex.Message}");
        }
    }
}
