using CatalogCloud.Application.DTOs;
using CatalogCloud.Application.Services;
using CatalogCloud.Domain.Entities;
using CatalogCloud.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace CatalogCloud.Tests.Unit;

public class AuthorServiceTests
{
    private readonly Mock<IAuthorRepository> _authorRepositoryMock;
    private readonly AuthorService _sut;

    public AuthorServiceTests()
    {
        _authorRepositoryMock = new Mock<IAuthorRepository>();
        _sut = new AuthorService(_authorRepositoryMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAuthorDto_WhenAuthorExists()
    {
        var id = "author-id";
        var author = new Author { Id = id, FullName = "Existing Author" };
        _authorRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _sut.GetByIdAsync(id);

        result.Should().NotBeNull();
        result!.Id.Should().Be(id);
        result.FullName.Should().Be("Existing Author");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenAuthorDoesNotExist()
    {
        _authorRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _sut.GetByIdAsync("missing-id");

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.GetByIdAsync(" ");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnMappedAuthors()
    {
        _authorRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new[] { new Author { Id = "one", FullName = "First Author" } });

        var result = await _sut.GetAllAsync();

        result.Should().ContainSingle(x => x.Id == "one" && x.FullName == "First Author");
    }

    [Fact]
    public async Task CreateAsync_ShouldReturnCreatedAuthorDto()
    {
        var dto = CreateValidAuthorDto("New Author");

        var result = await _sut.CreateAsync(dto);

        result.Id.Should().NotBeNullOrWhiteSpace();
        result.FullName.Should().Be(dto.FullName);
        result.IsActive.Should().BeTrue();
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _authorRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnFalse_WhenAuthorDoesNotExist()
    {
        _authorRepositoryMock.Setup(x => x.GetByIdAsync("missing-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _sut.UpdateAsync("missing-id", CreateValidUpdateAuthorDto("Updated Author"));

        result.Should().BeFalse();
        _authorRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Author>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.UpdateAsync(string.Empty, CreateValidUpdateAuthorDto("Updated Author"));

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnTrue_WhenAuthorIsUpdated()
    {
        var author = new Author { Id = "author-id", FullName = "Old Author", IsActive = true };
        var dto = CreateValidUpdateAuthorDto("Updated Author");
        _authorRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _sut.UpdateAsync(author.Id, dto);

        result.Should().BeTrue();
        author.FullName.Should().Be(dto.FullName);
        author.IsActive.Should().BeFalse();
        _authorRepositoryMock.Verify(x => x.UpdateAsync(author, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenAuthorDoesNotExist()
    {
        _authorRepositoryMock.Setup(x => x.GetByIdAsync("missing-id", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Author?)null);

        var result = await _sut.DeleteAsync("missing-id");

        result.Should().BeFalse();
        _authorRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldThrowArgumentException_WhenIdIsEmpty()
    {
        Func<Task> act = async () => await _sut.DeleteAsync(" ");

        await act.Should().ThrowAsync<ArgumentException>()
            .WithParameterName("id");
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenAuthorExists()
    {
        var author = new Author { Id = "author-id" };
        _authorRepositoryMock.Setup(x => x.GetByIdAsync(author.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(author);

        var result = await _sut.DeleteAsync(author.Id);

        result.Should().BeTrue();
        _authorRepositoryMock.Verify(x => x.DeleteAsync(author.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static CreateAuthorDto CreateValidAuthorDto(string fullName)
    {
        return new CreateAuthorDto
        {
            FullName = fullName,
            Biography = "Biography with enough details.",
            BirthDate = new DateTime(1975, 2, 2),
            PublishedBooksCount = 2,
            IsActive = true
        };
    }

    private static UpdateAuthorDto CreateValidUpdateAuthorDto(string fullName)
    {
        return new UpdateAuthorDto
        {
            FullName = fullName,
            Biography = "Updated biography with enough details.",
            BirthDate = new DateTime(1980, 3, 3),
            PublishedBooksCount = 4,
            IsActive = false
        };
    }
}
