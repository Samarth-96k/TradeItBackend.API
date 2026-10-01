using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradeIt.API.DTOs;
using TradeIt.API.Services;

namespace TradeIt.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? search,
        [FromQuery] string? category)
    {
        var products = await _service.GetAllAsync(search, category);

        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _service.GetByIdAsync(id);

        if (product == null)
            return NotFound(new
            {
                message = "Product not found."
            });

        return Ok(product);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductDto dto)
    {
        var sellerId = GetUserId();

        var product = await _service.CreateAsync(dto, sellerId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        UpdateProductDto dto)
    {
        var sellerId = GetUserId();

        var success = await _service.UpdateAsync(
            id,
            dto,
            sellerId);

        if (!success)
            return NotFound(new
            {
                message = "Product not found or you are not the seller."
            });

        return Ok(new
        {
            message = "Product updated successfully."
        });
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var sellerId = GetUserId();

        var success = await _service.DeleteAsync(
            id,
            sellerId);

        if (!success)
            return NotFound(new
            {
                message = "Product not found or you are not the seller."
            });

        return Ok(new
        {
            message = "Product deleted successfully."
        });
    }

    private int GetUserId()
    {
        return int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    }
}