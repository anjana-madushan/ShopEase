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

  private readonly EmailService _emailService;

  public AdminController(MongoDBService mongoDBService, IOptions<JwtSettings> jwtSettings, PasswordService passwordService, EmailService emailService)
  {
    _mongoDBService = mongoDBService;
    _jwtSettings = jwtSettings.Value;
    _passwordService = passwordService;
    _emailService = emailService;
  }

  //Create a new Admin user call the DTO
  [HttpPost]
  public async Task<IActionResult> Post([FromBody] AdminDTO admin)
  {
    try
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
    catch (Exception ex)
    {
      return StatusCode(500, $"An internal server error occurred: {ex.Message}");
    }
  }

  //Get Admin by ID
  [HttpGet("{id}")]
  public async Task<ActionResult<Admin>> Get(string id)
  {
    try
    {
      var admin = await _mongoDBService.GetAdminByIdAsync(id);

      if (admin is null)
      {
        return NotFound();
      }

      return admin;
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An internal server error occurred: {ex.Message}");
    }
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
      //Check if an CSR with the provided email already exists
      var existingCSR = await _mongoDBService.GetCSRByEmailAsync(admin.Email);
      if (existingCSR != null)
      {
        return Conflict("A CSR with the provided email already exists.");
      }
      //Check if an Vendor with the provided email already exists
      var existingVendor = await _mongoDBService.GetVendorByEmailAsync(admin.Email);
      if (existingVendor != null)
      {
        return Conflict("A Vendor with the provided email already exists.");
      }
      //Check if an Customer with the provided email already exists
      var existingUser = await _mongoDBService.GetCustomerByEmailAsync(admin.Email);
      if (existingUser != null)
      {
        return Conflict("A User with the provided email already exists.");
      }

      // Hash the password
      string hashedPassword = _passwordService.HashPassword(admin.Password);
      var newAdmin = new Admin
      {
        Username = admin.Username,
        Password = hashedPassword,
        Email = admin.Email
      };

      var responseAdmin = new Admin
      {
        Username = admin.Username,
        Email = admin.Email
      };

      await _mongoDBService.CreateAdminAsync(newAdmin);

      //Find the admin by id and update the admin List
      adminList.AdminsCreated.Add(newAdmin);
      await _mongoDBService.UpdateAdminAsync(adminList.Id, adminList);

      //Send email to Admin with the password and email
      await _emailService.SendEmailAsync(
newAdmin.Email,
"Welcome to the team",
$"Your Email is {admin.Email}\nPassword: {admin.Password}\nPlease change your password after login."
);

      return CreatedAtAction(nameof(Get), new { id = newAdmin.Id }, responseAdmin);
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

      // Check if an csr with the provided email already exists
      var existingCSR = await _mongoDBService.GetCSRByEmailAsync(csr.Email);
      if (existingCSR != null)
      {
        return Conflict("A CSR with the provided email already exists.");
      }

      //Check if an Vendor with the provided email already exists
      var existingVendor = await _mongoDBService.GetVendorByEmailAsync(csr.Email);
      if (existingVendor != null)
      {
        return Conflict("A Vendor with the provided email already exists.");
      }

      //Check if an Customer with the provided email already exists
      var existingUser = await _mongoDBService.GetCustomerByEmailAsync(csr.Email);
      if (existingUser != null)
      {
        return Conflict("A User with the provided email already exists.");
      }

      //Check if an Admin with the provided email already exists
      var existingAdmin = await _mongoDBService.GetAdminByEmailAsync(csr.Email);
      if (existingAdmin != null)
      {
        return Conflict("An Admin with the provided email already exists.");
      }

      // Hash the password
      string hashedPassword = _passwordService.HashPassword(csr.Password);
      var newCSR = new CSR
      {
        Username = csr.Username,
        Password = hashedPassword,
        Email = csr.Email
      };

      var responseCSR = new CSR
      {
        Username = csr.Username,
        Password = hashedPassword,
        Email = csr.Email
      };

      await _mongoDBService.CreateCSRAsync(newCSR);

      //Send email to CSR with the password and email
      await _emailService.SendEmailAsync(newCSR.Email, "Welcome to the team", $"Your Email is ${csr.Email} and password: {csr.Password}");
      //Find the admin by id and update the admin List
      adminList.CSRCreated.Add(newCSR);
      await _mongoDBService.UpdateAdminAsync(adminList.Id, adminList);

      return CreatedAtAction(nameof(Get), new { id = newCSR.Id }, responseCSR);
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

      var resVendor = new Vendor
      {
        Username = vendor.Username,
        Email = vendor.Email
      };


      await _mongoDBService.CreateVendorAsync(newVendor);

      //Send email to Vendor with the password and email
      await _emailService.SendEmailAsync(newVendor.Email, "Welcome to the team", $"Your Email is ${vendor.Email} and password: {vendor.Password}");

      //Find the admin by id and update the admin List
      adminList.VendorCreated.Add(newVendor);
      await _mongoDBService.UpdateAdminAsync(adminList.Id, adminList);

      return CreatedAtAction(nameof(Get), new { id = newVendor.Id }, resVendor);
    }
    catch (Exception ex)
    {
      return StatusCode(500, $"An internal server error occurred: {ex.Message}");
    }
  }
}