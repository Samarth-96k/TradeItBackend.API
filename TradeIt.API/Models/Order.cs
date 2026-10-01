namespace TradeIt.API.Models;

public class Order
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int BuyerId { get; set; }

    public User? Buyer { get; set; }

    public int SellerId { get; set; }

    public User? Seller { get; set; }

    public decimal FinalPrice { get; set; }

    public string Status { get; set; } = "Pending";

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
}