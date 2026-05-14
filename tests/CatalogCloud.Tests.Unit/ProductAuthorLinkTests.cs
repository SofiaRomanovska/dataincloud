using CatalogCloud.Domain.Entities;
using FluentAssertions;

namespace CatalogCloud.Tests.Unit;

public class ProductAuthorLinkTests
{
    [Fact]
    public void Constructor_ShouldCreateLink_WhenValuesAreValid()
    {
        var productId = Guid.NewGuid();

        var link = new ProductAuthorLink(productId, "author-id");

        link.ProductId.Should().Be(productId);
        link.AuthorId.Should().Be("author-id");
        link.CachedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void Constructor_ShouldRejectEmptyProductId()
    {
        var act = () => new ProductAuthorLink(Guid.Empty, "author-id");

        act.Should().Throw<ArgumentException>()
            .WithParameterName("productId");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Constructor_ShouldRejectMissingAuthorId(string authorId)
    {
        var act = () => new ProductAuthorLink(Guid.NewGuid(), authorId);

        act.Should().Throw<ArgumentException>()
            .WithParameterName("authorId");
    }

    [Fact]
    public void ChangeAuthor_ShouldUpdateAuthorAndCachedAt()
    {
        var link = new ProductAuthorLink(Guid.NewGuid(), "old-author-id");
        var originalCachedAt = link.CachedAt;

        link.ChangeAuthor("new-author-id");

        link.AuthorId.Should().Be("new-author-id");
        link.CachedAt.Should().BeOnOrAfter(originalCachedAt);
    }
}
