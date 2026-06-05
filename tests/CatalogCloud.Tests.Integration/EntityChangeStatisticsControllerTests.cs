using CatalogCloud.API.Contracts;
using CatalogCloud.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CatalogCloud.Tests.Integration;

public class EntityChangeStatisticsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EntityChangeStatisticsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_ShouldReturnStatistics_ForCreatedAndUpdatedEntities()
    {
        var product = await CreateProductAsync("Stats Product");
        var author = await CreateAuthorAsync("Stats Author");

        var productUpdateResponse = await _client.PutAsJsonAsync(
            $"/api/products/{product.Id}",
            new UpdateProductDto
            {
                Name = "Stats Product Updated",
                Description = "Updated product description for statistics test.",
                Price = 55.25m,
                QuantityInStock = 6
            });
        productUpdateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var authorUpdateResponse = await _client.PutAsJsonAsync(
            $"/api/authors/{author.Id}",
            new UpdateAuthorDto
            {
                FullName = "Stats Author Updated",
                Biography = "Updated biography that remains valid for statistics coverage.",
                BirthDate = new DateTime(1984, 8, 15),
                PublishedBooksCount = 9,
                IsActive = false
            });
        authorUpdateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var linkCreateResponse = await _client.PostAsJsonAsync(
            "/api/product-author-links",
            new CreateProductAuthorLinkDto
            {
                ProductId = product.Id,
                AuthorId = author.Id
            });
        linkCreateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var secondAuthor = await CreateAuthorAsync("Stats Second Author");
        var linkUpdateResponse = await _client.PutAsJsonAsync(
            $"/api/product-author-links/{product.Id}",
            new UpdateProductAuthorLinkDto
            {
                AuthorId = secondAuthor.Id
            });
        linkUpdateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var allStatistics = await GetStatisticsAsync("/api/statistics/entity-changes");
        allStatistics.Items.Should().ContainEquivalentOf(new EntityChangeStatisticItemResponse
        {
            EntityType = "Product",
            Operation = "Created",
            Count = 1
        });
        allStatistics.Items.Should().ContainEquivalentOf(new EntityChangeStatisticItemResponse
        {
            EntityType = "Product",
            Operation = "Updated",
            Count = 1
        });
        allStatistics.Items.Should().ContainEquivalentOf(new EntityChangeStatisticItemResponse
        {
            EntityType = "Author",
            Operation = "Created",
            Count = 2
        });
        allStatistics.Items.Should().ContainEquivalentOf(new EntityChangeStatisticItemResponse
        {
            EntityType = "Author",
            Operation = "Updated",
            Count = 1
        });
        allStatistics.Items.Should().ContainEquivalentOf(new EntityChangeStatisticItemResponse
        {
            EntityType = "ProductAuthorLink",
            Operation = "Created",
            Count = 1
        });
        allStatistics.Items.Should().ContainEquivalentOf(new EntityChangeStatisticItemResponse
        {
            EntityType = "ProductAuthorLink",
            Operation = "Updated",
            Count = 1
        });

        var filteredStatistics = await GetStatisticsAsync(
            "/api/statistics/entity-changes?entityType=ProductAuthorLink&operation=Updated");
        var filteredItems = filteredStatistics.Items.ToArray();
        filteredItems.Should().HaveCount(1);
        filteredItems[0].EntityType.Should().Be("ProductAuthorLink");
        filteredItems[0].Operation.Should().Be("Updated");
        filteredItems[0].Count.Should().Be(1);
    }

    private async Task<EntityChangeStatisticsResponse> GetStatisticsAsync(string url)
    {
        var response = await _client.GetAsync(url);
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return (await response.Content.ReadFromJsonAsync<EntityChangeStatisticsResponse>())!;
    }

    private async Task<ProductDto> CreateProductAsync(string name)
    {
        var response = await _client.PostAsJsonAsync(
            "/api/products",
            new CreateProductDto
            {
                Name = name,
                Description = "Product created for entity change statistics coverage.",
                Price = 42.10m,
                QuantityInStock = 5
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
                Biography = "Biography created for entity change statistics coverage.",
                BirthDate = new DateTime(1978, 7, 7),
                PublishedBooksCount = 4,
                IsActive = true
            });
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthorDto>())!;
    }
}
