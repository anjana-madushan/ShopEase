using System;
using System.Collections.Generic;

namespace server.DTOs
{

    public class ProductDetailsDTO
    {
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDTO
    {
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }
        public string Email { get; set; }
        public Dictionary<string, ProductDetailsDTO> Products { get; set; } = new Dictionary<string, ProductDetailsDTO>();
        public decimal TotalPrice { get; set; }
        public int TotalQty { get; set; }
        public string UserId { get; set; }

       
    }
}
