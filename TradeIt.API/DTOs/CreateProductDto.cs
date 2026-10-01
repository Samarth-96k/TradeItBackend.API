using System.ComponentModel.DataAnnotations;

namespace TradeIt.API.DTOs;

public class CreateProductDto
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal OriginalPrice { get; set; }

    [Range(0, double.MaxValue)]
    public decimal SellingPrice { get; set; }

    [Required]
    public string Condition { get; set; } = string.Empty;

    [Range(0, 100)]
    public int ProductAge { get; set; }

    public string Location { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }
}