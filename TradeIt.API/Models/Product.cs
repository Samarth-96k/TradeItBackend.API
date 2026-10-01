namespace TradeIt.API.Models;

public class Product
{
    public int Id { get; set; }

    public int SellerId { get; set; }

    public User? Seller { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal OriginalPrice { get; set; }

    public decimal SellingPrice { get; set; }

    public string Condition { get; set; } = string.Empty;

    public int ProductAge { get; set; }

    public string Location { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public string Status { get; set; } = "Available";

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}