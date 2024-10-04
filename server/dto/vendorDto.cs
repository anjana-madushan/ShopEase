public class VendorDTO
{
    public string Username { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public List<ProductDTO> ProductsCreated { get; set; } = new List<ProductDTO>();
}
