using TradeIt.API.DTOs;
using TradeIt.API.Models;
using TradeIt.API.Repositories;

namespace TradeIt.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _repository;

    public ProductService(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<Product>> GetAllAsync(
        string? search,
        string? category)
    {
        return await _repository.GetAllAsync(search, category);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdWithSellerAsync(id);
    }

    public async Task<Product> CreateAsync(
        CreateProductDto dto,
        int sellerId)
    {
        var product = new Product
        {
            SellerId = sellerId,
            Name = dto.Name,
            Category = dto.Category,
            Description = dto.Description,
            OriginalPrice = dto.OriginalPrice,
            SellingPrice = dto.SellingPrice,
            Condition = dto.Condition,
            ProductAge = dto.ProductAge,
            Location = dto.Location,
            ImageUrl = dto.ImageUrl,
            Status = "Available"
        };

        return await _repository.AddAsync(product);
    }

    public async Task<bool> UpdateAsync(
        int id,
        UpdateProductDto dto,
        int sellerId)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null || product.SellerId != sellerId)
            return false;

        product.Name = dto.Name;
        product.Category = dto.Category;
        product.Description = dto.Description;
        product.OriginalPrice = dto.OriginalPrice;
        product.SellingPrice = dto.SellingPrice;
        product.Condition = dto.Condition;
        product.ProductAge = dto.ProductAge;
        product.Location = dto.Location;
        product.ImageUrl = dto.ImageUrl;
        product.Status = dto.Status;

        await _repository.UpdateAsync(product);

        return true;
    }

    public async Task<bool> DeleteAsync(
        int id,
        int sellerId)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product == null || product.SellerId != sellerId)
            return false;

        await _repository.DeleteAsync(product);

        return true;
    }
}
