using System.ComponentModel.DataAnnotations;

namespace TradeIt.API.DTOs;

public class CreateOfferDto
{
    [Required]
    public int ProductId { get; set; }

    [Range(1, double.MaxValue)]
    public decimal OfferPrice { get; set; }

    public string Message { get; set; } = string.Empty;
}