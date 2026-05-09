using Bookstore.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Bookstore.Tests.Integration;

public class ProductAuthorLinksControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductAuthorLinksControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldReturnCreated_WhenProductAndAuthorExist()
    {
        var product = await CreateProductAsync("Link Product");
        var author = await CreateAuthorAsync("Link Author");
        var dto = new CreateProductAuthorLinkDto
        {
            ProductId = product.Id,
            AuthorId = author.Id
        };

        var response = await _client.PostAsJsonAsync("/api/product-author-links", dto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var link = await response.Content.ReadFromJsonAsync<ProductAuthorLinkDto>();
        link.Should().NotBeNull();
        link!.ProductId.Should().Be(product.Id);
        link.AuthorId.Should().Be(author.Id);
    }

    [Fact]
    public async Task Create_ShouldReturnNotFound_WhenProductDoesNotExist()
    {
        var author = await CreateAuthorAsync("Author Without Product");
        var dto = new CreateProductAuthorLinkDto
        {
            ProductId = Guid.NewGuid(),
            AuthorId = author.Id
        };

        var response = await _client.PostAsJsonAsync("/api/product-author-links", dto);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_ShouldReturnOk_WhenLinkAndNewAuthorExist()
    {
        var product = await CreateProductAsync("Update Link Product");
        var firstAuthor = await CreateAuthorAsync("First Link Author");
        var secondAuthor = await CreateAuthorAsync("Second Link Author");
        await CreateLinkAsync(product.Id, firstAuthor.Id);

        var response = await _client.PutAsJsonAsync(
            $"/api/product-author-links/{product.Id}",
            new UpdateProductAuthorLinkDto { AuthorId = secondAuthor.Id });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var link = await response.Content.ReadFromJsonAsync<ProductAuthorLinkDto>();
        link.Should().NotBeNull();
        link!.AuthorId.Should().Be(secondAuthor.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFound_WhenLinkDoesNotExist()
    {
        var author = await CreateAuthorAsync("Unlinked Author");

        var response = await _client.PutAsJsonAsync(
            $"/api/product-author-links/{Guid.NewGuid()}",
            new UpdateProductAuthorLinkDto { AuthorId = author.Id });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenLinkExists()
    {
        var product = await CreateProductAsync("Delete Link Product");
        var author = await CreateAuthorAsync("Delete Link Author");
        await CreateLinkAsync(product.Id, author.Id);

        var response = await _client.DeleteAsync($"/api/product-author-links/{product.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenLinkDoesNotExist()
    {
        var response = await _client.DeleteAsync($"/api/product-author-links/{Guid.NewGuid()}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<ProductAuthorLinkDto> CreateLinkAsync(Guid productId, string authorId)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/product-author-links",
            new CreateProductAuthorLinkDto { ProductId = productId, AuthorId = authorId });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ProductAuthorLinkDto>())!;
    }

    private async Task<ProductDto> CreateProductAsync(string name)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductDto
            {
                Name = name,
                Description = "Product created for product-author link tests.",
                Price = 15.50m,
                QuantityInStock = 4
            });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<ProductDto>())!;
    }

    private async Task<AuthorDto> CreateAuthorAsync(string fullName)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/authors",
            new CreateAuthorDto
            {
                FullName = fullName,
                Biography = "Biography created for product-author link tests.",
                BirthDate = new DateTime(1982, 4, 11),
                PublishedBooksCount = 8,
                IsActive = true
            });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthorDto>())!;
    }
}
