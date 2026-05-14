namespace CatalogCloud.Domain.Entities;

public class Product
{
    public const int MaxNameLength = 200;
    public const int MaxDescriptionLength = 500;
    public const decimal MaxPrice = 1_000_000;
    public const int MaxQuantityInStock = 100_000;

    private Product()
    {
    }

    public Product(string name, string description, decimal price, int quantityInStock)
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;

        UpdateDetails(name, description, price, quantityInStock);
    }

    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int QuantityInStock { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public bool IsDeleted { get; private set; }

    public void UpdateDetails(string name, string description, decimal price, int quantityInStock)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Product name is required.", nameof(name));
        }

        if (name.Length > MaxNameLength)
        {
            throw new ArgumentException($"Product name cannot exceed {MaxNameLength} characters.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(description))
        {
            throw new ArgumentException("Product description is required.", nameof(description));
        }

        if (description.Length > MaxDescriptionLength)
        {
            throw new ArgumentException($"Product description cannot exceed {MaxDescriptionLength} characters.", nameof(description));
        }

        if (price <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), price, "Product price must be greater than zero.");
        }

        if (price > MaxPrice)
        {
            throw new ArgumentOutOfRangeException(nameof(price), price, $"Product price cannot exceed {MaxPrice}.");
        }

        if (quantityInStock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityInStock), quantityInStock, "Product quantity cannot be negative.");
        }

        if (quantityInStock > MaxQuantityInStock)
        {
            throw new ArgumentOutOfRangeException(nameof(quantityInStock), quantityInStock, $"Product quantity cannot exceed {MaxQuantityInStock}.");
        }

        Name = name;
        Description = description;
        Price = price;
        QuantityInStock = quantityInStock;
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}
