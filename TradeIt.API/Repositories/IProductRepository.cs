using TradeIt.API.Models;

namespace TradeIt.API.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync(
        string? search = null,
        string? category = null);

    Task<Product?> GetByIdAsync(int id);

    Task<Product?> GetByIdWithSellerAsync(int id);

    Task<Product> AddAsync(Product product);

    Task UpdateAsync(Product product);

    Task DeleteAsync(Product product);
}