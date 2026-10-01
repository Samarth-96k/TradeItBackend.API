using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeIt.API.Data;
using TradeIt.API.DTOs;
using TradeIt.API.Models;

namespace TradeIt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReviewsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetProductReviews(int productId)
    {
        var reviews = await _context.Reviews
            .Where(x => x.ProductId == productId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(reviews);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewDto dto)
    {
        var buyerId = GetUserId();

        var order = await _context.Orders
            .FirstOrDefaultAsync(x =>
                x.ProductId == dto.ProductId &&
                x.BuyerId == buyerId &&
                x.Status == "Completed");

        if (order == null)
        {
            return BadRequest(new
            {
                message = "You can review only products you purchased."
            });
        }

        var alreadyReviewed = await _context.Reviews
            .AnyAsync(x =>
                x.ProductId == dto.ProductId &&
                x.BuyerId == buyerId);

        if (alreadyReviewed)
        {
            return BadRequest(new
            {
                message = "You already reviewed this product."
            });
        }

        var review = new Review
        {
            ProductId = dto.ProductId,
            BuyerId = buyerId,
            SellerId = order.SellerId,
            Rating = dto.Rating,
            Comment = dto.Comment
        };

        _context.Reviews.Add(review);

        await _context.SaveChangesAsync();

        return Ok(review);
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}