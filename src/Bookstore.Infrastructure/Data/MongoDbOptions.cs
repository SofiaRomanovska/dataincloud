namespace Bookstore.Infrastructure.Data;

public class MongoDbOptions
{
    public const string SectionName = "MongoDb";

    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = "BookstoreAuthorsDb";
    public string AuthorsCollectionName { get; set; } = "authors";
}
