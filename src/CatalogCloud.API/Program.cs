using CatalogCloud.Application;
using CatalogCloud.Infrastructure;
using CatalogCloud.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<InMemoryEntityChangeStatisticsStore>();
builder.Services.AddHostedService<EntityChangeStatisticsSubscriber>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.Services.ApplyInfrastructureMigrationsAsync();

app.Run();

public partial class Program { }
