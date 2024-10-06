using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace server.Models;

// Defined an enum for OrderStatus
public enum OrderStatus
{
    Processing,
    Ready,
    Dispatched,
    Delivered,
    Cancelled
}

// Defined a class for ProductDetails with Price and Quantity properties
public class ProductDetails
{
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}


public class Order
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    [BsonElement("orderId")]
    public string OrderId { get; set; }

    [BsonElement("orderDate")]
    public string OrderDate { get; set; }

    [BsonElement("Status")]
    public OrderStatus Status { get; set; }

    [BsonElement("statusUpdatedOn")]
    public string StatusUpdatedOn { get; set; }

    [BsonElement("shippingAddress")]
    public string ShippingAddress { get; set; }

    [BsonElement("billingAddress")]
    public string BillingAddress { get; set; }

    [BsonElement("email")]
    public string Email { get; set; }

    [BsonElement("products")]
    public Dictionary<string, ProductDetails> Products { get; set; } = new Dictionary<string, ProductDetails>();

    [BsonElement("totalPrice")]
    public decimal TotalPrice { get; set; }


    [BsonElement("totalQty")]
    public int TotalQty { get; set; }

    [BsonElement("requestToCancel")]
    public bool? RequestToCancel { get; set; } = false;

    [BsonElement("cancelled")]
    public bool? Cancelled { get; set; } = false;

    [BsonElement("cancelledOn")]
    public string? CancelledOn { get; set; }

    [BsonElement("cancelledBy")]
    public string? CancelledBy { get; set; }

    [BsonElement("note")]
    public string? Note { get; set; } = "";

    [BsonElement("[paymentStatus]")]
    public bool PaymentStatus { get; set; } = false;


    [BsonElement("userId")]
    public string UserId { get; set; }


}