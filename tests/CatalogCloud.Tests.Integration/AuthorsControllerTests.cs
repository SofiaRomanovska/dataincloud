using CatalogCloud.Application.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CatalogCloud.Tests.Integration;

public class AuthorsControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthorsControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAuthor()
    {
        var createDto = CreateValidAuthorDto("Created Author");

        var response = await _client.PostAsJsonAsync("/api/authors", createDto);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var author = await response.Content.ReadFromJsonAsync<AuthorDto>();
        author.Should().NotBeNull();
        author!.Id.Should().NotBeNullOrWhiteSpace();
        author.FullName.Should().Be(createDto.FullName);
        author.PublishedBooksCount.Should().Be(createDto.PublishedBooksCount);
    }

    [Fact]
    public async Task GetAll_ShouldReturnAuthors()
    {
        await CreateAuthorAsync("List Author");

        var response = await _client.GetAsync("/api/authors");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var authors = await response.Content.ReadFromJsonAsync<IEnumerable<AuthorDto>>();
        authors.Should().NotBeNull();
        authors.Should().Contain(x => x.FullName == "List Author");
    }

    [Fact]
    public async Task GetById_ShouldReturnAuthor_WhenAuthorExists()
    {
        var created = await CreateAuthorAsync("Readable Author");

        var response = await _client.GetAsync($"/api/authors/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var author = await response.Content.ReadFromJsonAsync<AuthorDto>();
        author.Should().NotBeNull();
        author!.Id.Should().Be(created.Id);
    }

    [Fact]
    public async Task Update_ShouldReturnNoContent_WhenAuthorExists()
    {
        var created = await CreateAuthorAsync("Original Author");
        var updateDto = new UpdateAuthorDto
        {
            FullName = "Updated Author",
            Biography = "Updated biography that is long enough for validation.",
            BirthDate = new DateTime(1985, 6, 15),
            PublishedBooksCount = 12,
            IsActive = false
        };

        var response = await _client.PutAsJsonAsync($"/api/authors/{created.Id}", updateDto);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync($"/api/authors/{created.Id}");
        var author = await getResponse.Content.ReadFromJsonAsync<AuthorDto>();
        author!.FullName.Should().Be(updateDto.FullName);
        author.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenAuthorExists()
    {
        var created = await CreateAuthorAsync("Deleted Author");

        var response = await _client.DeleteAsync($"/api/authors/{created.Id}");

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var getResponse = await _client.GetAsync($"/api/authors/{created.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenPayloadIsInvalid()
    {
        var createDto = CreateValidAuthorDto(string.Empty);

        var response = await _client.PostAsJsonAsync("/api/authors", createDto);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    private async Task<AuthorDto> CreateAuthorAsync(string fullName)
    {
        var response = await _client.PostAsJsonAsync("/api/authors", CreateValidAuthorDto(fullName));
        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<AuthorDto>())!;
    }

    private static CreateAuthorDto CreateValidAuthorDto(string fullName)
    {
        return new CreateAuthorDto
        {
            FullName = fullName,
            Biography = "A detailed biography that satisfies the validation rules.",
            BirthDate = new DateTime(1970, 1, 20),
            PublishedBooksCount = 5,
            IsActive = true
        };
    }
}
