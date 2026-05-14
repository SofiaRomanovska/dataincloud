using System.ComponentModel.DataAnnotations;

namespace CatalogCloud.API.Contracts;

public class GetPagedProductsRequest
{
    [Range(1, int.MaxValue)]
    public int Page { get; init; } = 1;

    [Range(1, int.MaxValue)]
    public int PageSize { get; init; } = 10;
}
