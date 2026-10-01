namespace TradeIt.API.Models;

public class Offer
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int BuyerId { get; set; }

    public User? Buyer { get; set; }

    public decimal OfferPrice { get; set; }

    public string Message { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}