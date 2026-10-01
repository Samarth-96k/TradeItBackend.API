using TradeIt.API.DTOs;
using TradeIt.API.Models;

namespace TradeIt.API.Services;

public interface IProductService
{
    Task<List<Product>> GetAllAsync(
        string? search,
        string? category);

    Task<Product?> GetByIdAsync(int id);

    Task<Product> CreateAsync(
        CreateProductDto dto,
        int sellerId);

    Task<bool> UpdateAsync(
        int id,
        UpdateProductDto dto,
        int sellerId);

    Task<bool> DeleteAsync(
        int id,
        int sellerId);
}