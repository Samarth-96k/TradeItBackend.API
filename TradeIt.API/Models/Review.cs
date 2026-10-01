namespace TradeIt.API.Models;

public class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int BuyerId { get; set; }

    public int SellerId { get; set; }

    public int Rating { get; set; }

    public string Comment { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}