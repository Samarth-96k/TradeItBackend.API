using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TradeIt.API.Data;
using TradeIt.API.DTOs;

namespace TradeIt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrdersController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================
    // BUY PRODUCT
    // POST: api/Orders
    // =========================================================
    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] CreateOrderDto dto)
    {
        var buyerId = GetUserId();

        // Find product
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.Id == dto.ProductId);

        if (product == null)
        {
            return NotFound(new
            {
                message = "Product not found."
            });
        }

        // Check whether product is available
        if (product.Status != "Available")
        {
            return BadRequest(new
            {
                message = "This product is no longer available."
            });
        }

        // Seller cannot buy his own product
        if (product.SellerId == buyerId)
        {
            return BadRequest(new
            {
                message = "You cannot buy your own product."
            });
        }

        // Create order
        var order = new Models.Order
        {
            ProductId = product.Id,
            BuyerId = buyerId,
            SellerId = product.SellerId,
            Status = "Pending",
            OrderDate = DateTime.UtcNow
        };

        _context.Orders.Add(order);

        // Mark product as sold
        product.Status = "Sold";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Product purchased successfully.",
            orderId = order.Id,
            productId = product.Id,
            buyerId = buyerId,
            sellerId = product.SellerId,
            status = order.Status
        });
    }


    // =========================================================
    // GET MY ORDERS
    // GET: api/Orders/my
    // =========================================================
    [HttpGet("my")]
    public async Task<IActionResult> MyOrders()
    {
        var userId = GetUserId();

        var orders = await _context.Orders
            .Include(x => x.Product)
            .Where(x =>
                x.BuyerId == userId ||
                x.SellerId == userId)
            .OrderByDescending(x => x.OrderDate)
            .Select(x => new
            {
                x.Id,
                x.ProductId,
                ProductName = x.Product!.Name,
                x.BuyerId,
                x.SellerId,
                x.Status,
                x.OrderDate
            })
            .ToListAsync();

        return Ok(orders);
    }


    // =========================================================
    // GET ORDER BY ID
    // GET: api/Orders/{id}
    // =========================================================
    [HttpGet("{id}")]
    public async Task<IActionResult> GetOrder(int id)
    {
        var userId = GetUserId();

        var order = await _context.Orders
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            return NotFound(new
            {
                message = "Order not found."
            });
        }

        // Only buyer or seller can see the order
        if (order.BuyerId != userId &&
            order.SellerId != userId)
        {
            return Forbid();
        }

        return Ok(new
        {
            order.Id,
            order.ProductId,
            ProductName = order.Product?.Name,
            order.BuyerId,
            order.SellerId,
            order.Status,
            order.OrderDate
        });
    }


    // =========================================================
    // COMPLETE ORDER
    // PUT: api/Orders/{id}/complete
    // =========================================================
    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(int id)
    {
        var userId = GetUserId();

        var order = await _context.Orders
            .FirstOrDefaultAsync(x => x.Id == id);

        if (order == null)
        {
            return NotFound(new
            {
                message = "Order not found."
            });
        }

        // Only buyer or seller can complete order
        if (order.BuyerId != userId &&
            order.SellerId != userId)
        {
            return Forbid();
        }

        order.Status = "Completed";

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Order completed successfully."
        });
    }


    // =========================================================
    // GET USER ID FROM JWT
    // =========================================================
    private int GetUserId()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
        {
            throw new UnauthorizedAccessException(
                "User ID not found in token.");
        }

        return int.Parse(userId);
    }
}