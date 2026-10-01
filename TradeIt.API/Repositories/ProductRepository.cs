using Microsoft.EntityFrameworkCore;
using TradeIt.API.Data;
using TradeIt.API.Models;

namespace TradeIt.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync(
        string? search = null,
        string? category = null)
    {
        var query = _context.Products
            .Include(x => x.Seller)
            .AsQueryable();

        query = query.Where(x => x.Status == "Available");

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.Name.Contains(search) ||
                x.Description.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(x => x.Category == category);
        }

        return await query
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product?> GetByIdWithSellerAsync(int id)
    {
        return await _context.Products
            .Include(x => x.Seller)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Product> AddAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        return product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Product product)
    {
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
    }
}