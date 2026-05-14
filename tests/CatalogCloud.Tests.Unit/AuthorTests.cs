using CatalogCloud.Domain.Entities;
using FluentAssertions;

namespace CatalogCloud.Tests.Unit;

public class AuthorTests
{
    [Fact]
    public void Constructor_ShouldCreateAuthor_WhenValuesAreValid()
    {
        var author = CreateAuthor();

        author.Id.Should().NotBeNullOrWhiteSpace();
        author.FullName.Should().Be("Author Name");
        author.Biography.Should().Be("Biography with enough details.");
        author.BirthDate.Should().Be(new DateTime(1975, 2, 2));
        author.PublishedBooksCount.Should().Be(2);
        author.IsActive.Should().BeTrue();
        author.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void UpdateDetails_ShouldChangeAuthorDetails_WhenValuesAreValid()
    {
        var author = CreateAuthor();

        author.UpdateDetails("Updated Author", "Updated biography with enough details.", new DateTime(1980, 3, 3), 4, false);

        author.FullName.Should().Be("Updated Author");
        author.Biography.Should().Be("Updated biography with enough details.");
        author.BirthDate.Should().Be(new DateTime(1980, 3, 3));
        author.PublishedBooksCount.Should().Be(4);
        author.IsActive.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldRejectMissingFullName(string fullName)
    {
        var act = () => new Author(fullName, "Biography with enough details.", new DateTime(1975, 2, 2), 2, true);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("fullName");
    }

    [Fact]
    public void Constructor_ShouldRejectShortBiography()
    {
        var act = () => new Author("Author Name", "Too short", new DateTime(1975, 2, 2), 2, true);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("biography");
    }

    [Fact]
    public void Constructor_ShouldRejectFutureBirthDate()
    {
        var act = () => new Author("Author Name", "Biography with enough details.", DateTime.UtcNow.AddDays(1), 2, true);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("birthDate");
    }

    [Fact]
    public void Constructor_ShouldRejectNegativePublishedBooksCount()
    {
        var act = () => new Author("Author Name", "Biography with enough details.", new DateTime(1975, 2, 2), -1, true);

        act.Should().Throw<ArgumentOutOfRangeException>()
            .WithParameterName("publishedBooksCount");
    }

    private static Author CreateAuthor()
    {
        return new Author(
            "Author Name",
            "Biography with enough details.",
            new DateTime(1975, 2, 2),
            2,
            true);
    }
}
