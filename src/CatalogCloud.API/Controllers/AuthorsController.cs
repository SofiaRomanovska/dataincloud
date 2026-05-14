using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace CatalogCloud.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorsController : ControllerBase
{
    private readonly IAuthorService _authorService;
    private readonly IValidator<CreateAuthorDto> _createValidator;
    private readonly IValidator<UpdateAuthorDto> _updateValidator;

    public AuthorsController(
        IAuthorService authorService,
        IValidator<CreateAuthorDto> createValidator,
        IValidator<UpdateAuthorDto> updateValidator)
    {
        _authorService = authorService;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var authors = await _authorService.GetAllAsync(cancellationToken);
        return Ok(authors);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest();

        var author = await _authorService.GetByIdAsync(id, cancellationToken);
        if (author == null) return NotFound();

        return Ok(author);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await _createValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdAuthor = await _authorService.CreateAsync(dto, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdAuthor.Id }, createdAuthor);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateAuthorDto dto, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest();

        var validationResult = await _updateValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var result = await _authorService.UpdateAsync(id, dto, cancellationToken);
        if (!result) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(id)) return BadRequest();

        var result = await _authorService.DeleteAsync(id, cancellationToken);
        if (!result) return NotFound();

        return NoContent();
    }
}
