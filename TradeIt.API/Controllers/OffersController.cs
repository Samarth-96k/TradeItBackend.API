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
[Authorize]
public class OffersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OffersController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateOfferDto dto)
    {
        var buyerId = GetUserId();

        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

        if (product == null)
            return NotFound(new
            {
                message = "Product not found."
            });

        if (product.SellerId == buyerId)
            return BadRequest(new
            {
                message = "You cannot make an offer on your own product."
            });

        if (product.Status != "Available")
            return BadRequest(new
            {
                message = "Product is no longer available."
            });

        var offer = new Offer
        {
            ProductId = dto.ProductId,
            BuyerId = buyerId,
            OfferPrice = dto.OfferPrice,
            Message = dto.Message,
            Status = "Pending"
        };

        _context.Offers.Add(offer);

        await _context.SaveChangesAsync();

        return Ok(offer);
    }

    [HttpGet("my")]
    public async Task<IActionResult> MyOffers()
    {
        var buyerId = GetUserId();

        var offers = await _context.Offers
            .Include(x => x.Product)
            .Where(x => x.BuyerId == buyerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(offers);
    }

    [HttpGet("received")]
    public async Task<IActionResult> ReceivedOffers()
    {
        var sellerId = GetUserId();

        var offers = await _context.Offers
            .Include(x => x.Product)
            .Include(x => x.Buyer)
            .Where(x => x.Product!.SellerId == sellerId)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();

        return Ok(offers);
    }

    [HttpPut("{id}/accept")]
    public async Task<IActionResult> Accept(int id)
    {
        var sellerId = GetUserId();

        var offer = await _context.Offers
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (offer == null)
            return NotFound();

        if (offer.Product!.SellerId != sellerId)
            return Forbid();

        if (offer.Status != "Pending")
            return BadRequest(new
            {
                message = "Offer has already been processed."
            });

        offer.Status = "Accepted";

        offer.Product.Status = "Sold";

        var order = new Order
        {
            ProductId = offer.ProductId,
            BuyerId = offer.BuyerId,
            SellerId = sellerId,
            FinalPrice = offer.OfferPrice,
            Status = "Confirmed"
        };

        _context.Orders.Add(order);

        var otherOffers = await _context.Offers
            .Where(x =>
                x.ProductId == offer.ProductId &&
                x.Id != offer.Id &&
                x.Status == "Pending")
            .ToListAsync();

        foreach (var otherOffer in otherOffers)
        {
            otherOffer.Status = "Rejected";
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Offer accepted and order created.",
            orderId = order.Id
        });
    }

    [HttpPut("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var sellerId = GetUserId();

        var offer = await _context.Offers
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (offer == null)
            return NotFound();

        if (offer.Product!.SellerId != sellerId)
            return Forbid();

        offer.Status = "Rejected";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Offer rejected."
        });
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}