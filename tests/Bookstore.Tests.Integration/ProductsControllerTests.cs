using Bookstore.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Bookstore.Tests.Integration;

public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public ProductsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CRUD_Operations_ShouldWorkAsExpected()
    {
        // 1. Create a new product
        var createDto = new CreateProductDto
        {
            Name = "Integration Test Product",
            Description = "Product created by integration test",
            Price = 19.99m,
            QuantityInStock = 7
        };

        var createResponse = await _client.PostAsJsonAsync("/api/products", createDto);
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var createdProduct = await createResponse.Content.ReadFromJsonAsync<ProductDto>();
        createdProduct.Should().NotBeNull();
        createdProduct!.Name.Should().Be("Integration Test Product");
        
        var productId = createdProduct.Id;

        // 2. Get the created product by ID
        var getResponse = await _client.GetAsync($"/api/products/{productId}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var getProduct = await getResponse.Content.ReadFromJsonAsync<ProductDto>();
        getProduct.Should().NotBeNull();
        getProduct!.Id.Should().Be(productId);

        // 3. Update the product
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product Name",
            Description = "Updated product description",
            Price = 25.00m,
            QuantityInStock = 3
        };
        var putResponse = await _client.PutAsJsonAsync($"/api/products/{productId}", updateDto);
        putResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify update
        var updatedResponse = await _client.GetAsync($"/api/products/{productId}");
        var updatedProduct = await updatedResponse.Content.ReadFromJsonAsync<ProductDto>();
        updatedProduct!.Name.Should().Be("Updated Product Name");

        // 4. Get Paged List
        var pagedResponse = await _client.GetAsync("/api/products?page=1&pageSize=10");
        pagedResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var productsList = await pagedResponse.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        productsList.Should().NotBeEmpty();

        // 5. Delete the product (Soft Delete)
        var deleteResponse = await _client.DeleteAsync($"/api/products/{productId}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // 6. Verify Soft Delete (Get should return NotFound)
        var getAfterDeleteResponse = await _client.GetAsync($"/api/products/{productId}");
        getAfterDeleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("/api/products?page=0&pageSize=10")]
    [InlineData("/api/products?page=1&pageSize=0")]
    public async Task GetPaged_ShouldReturnBadRequest_WhenPagingParametersAreInvalid(string url)
    {
        var response = await _client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("/api/products/00000000-0000-0000-0000-000000000000")]
    [InlineData("/api/products/00000000-0000-0000-0000-000000000000", true)]
    public async Task InvalidEmptyId_ShouldReturnBadRequest(string url, bool useDelete = false)
    {
        var response = useDelete
            ? await _client.DeleteAsync(url)
            : await _client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenIdIsEmpty()
    {
        var updateDto = new UpdateProductDto
        {
            Name = "Updated Product Name",
            Description = "Updated product description",
            Price = 25.00m,
            QuantityInStock = 3
        };

        var response = await _client.PutAsJsonAsync(
            "/api/products/00000000-0000-0000-0000-000000000000",
            updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
