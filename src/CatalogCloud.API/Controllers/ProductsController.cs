using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Interfaces;
using CatalogCloud.API.Contracts;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CatalogCloud.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IValidator<CreateProductDto> _createValidator;
    private readonly IValidator<UpdateProductDto> _updateValidator;

    public ProductsController(
        IProductService productService,
        IValidator<CreateProductDto> createValidator,
        IValidator<UpdateProductDto> updateValidator)
    {
        _productService = productService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPaged([FromQuery] GetPagedProductsRequest request, CancellationToken cancellationToken = default)
    {
        var products = await _productService.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return BadRequest();

        var product = await _productService.GetByIdAsync(id, cancellationToken);
        if (product == null) return NotFound();

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdProduct = await _productService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdProduct.Id }, createdProduct);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return BadRequest();

        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _productService.UpdateAsync(id, dto, cancellationToken);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken = default)
    {
        if (id == Guid.Empty) return BadRequest();

        var result = await _productService.DeleteAsync(id, cancellationToken);
        if (!result) return NotFound();

        return NoContent();
    }
}
