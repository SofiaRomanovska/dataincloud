namespace CatalogCloud.Application.DTOs;

public class ProductAuthorLinkDto
{
    public Guid ProductId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CachedAt { get; set; }
}

public class CreateProductAuthorLinkDto
{
    public Guid ProductId { get; set; }
    public string AuthorId { get; set; } = string.Empty;
}

public class UpdateProductAuthorLinkDto
{
    public string AuthorId { get; set; } = string.Empty;
}

public enum ProductAuthorLinkStatus
{
    Success,
    ProductNotFound,
    AuthorNotFound,
    LinkNotFound
}

public class ProductAuthorLinkResult
{
    public ProductAuthorLinkStatus Status { get; set; }
    public ProductAuthorLinkDto? Link { get; set; }

    public static ProductAuthorLinkResult Success(ProductAuthorLinkDto? link = null)
    {
        return new ProductAuthorLinkResult
        {
            Status = ProductAuthorLinkStatus.Success,
            Link = link
        };
    }

    public static ProductAuthorLinkResult Failure(ProductAuthorLinkStatus status)
    {
        return new ProductAuthorLinkResult
        {
            Status = status
        };
    }
}
