using Bookstore.Application.DTOs;
using Bookstore.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Bookstore.API.Controllers;

[ApiController]
[Route("api/product-author-links")]
public class ProductAuthorLinksController : ControllerBase
{
    private readonly IProductAuthorLinkService _linkService;
    private readonly IValidator<CreateProductAuthorLinkDto> _createValidator;
    private readonly IValidator<UpdateProductAuthorLinkDto> _updateValidator;

    public ProductAuthorLinksController(
        IProductAuthorLinkService linkService,
        IValidator<CreateProductAuthorLinkDto> createValidator,
        IValidator<UpdateProductAuthorLinkDto> updateValidator)
    {
        _linkService = linkService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductAuthorLinkDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _linkService.CreateAsync(dto, cancellationToken);
        return ToActionResult(result, created: true);
    }

    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> Update(
        Guid productId,
        [FromBody] UpdateProductAuthorLinkDto dto,
        CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) return BadRequest();

        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _linkService.UpdateAsync(productId, dto, cancellationToken);
        return ToActionResult(result);
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> Delete(Guid productId, CancellationToken cancellationToken = default)
    {
        if (productId == Guid.Empty) return BadRequest();

        var result = await _linkService.DeleteAsync(productId, cancellationToken);
        return ToActionResult(result);
    }

    private IActionResult ToActionResult(ProductAuthorLinkResult result, bool created = false)
    {
        return result.Status switch
        {
            ProductAuthorLinkStatus.Success when created => Created(string.Empty, result.Link),
            ProductAuthorLinkStatus.Success when result.Link != null => Ok(result.Link),
            ProductAuthorLinkStatus.Success => NoContent(),
            ProductAuthorLinkStatus.ProductNotFound => NotFound("Product was not found."),
            ProductAuthorLinkStatus.AuthorNotFound => NotFound("Author was not found."),
            ProductAuthorLinkStatus.LinkNotFound => NotFound("Product-author link was not found."),
            _ => BadRequest()
        };
    }
}
