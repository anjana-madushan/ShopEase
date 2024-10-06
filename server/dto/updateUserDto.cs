using System.ComponentModel.DataAnnotations;

public class UserUpdateDTO
{
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Role is required.")]
    public string Role { get; set; }

    public string? Username { get; set; }
    public string? Password { get; set; }
}
