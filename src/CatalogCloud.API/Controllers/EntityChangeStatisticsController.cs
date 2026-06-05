using CatalogCloud.API.Contracts;
using CatalogCloud.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace CatalogCloud.API.Controllers;

[ApiController]
[Route("api/statistics/entity-changes")]
public class EntityChangeStatisticsController : ControllerBase
{
    private readonly InMemoryEntityChangeStatisticsStore _statisticsStore;

    public EntityChangeStatisticsController(InMemoryEntityChangeStatisticsStore statisticsStore)
    {
        _statisticsStore = statisticsStore;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] GetEntityChangeStatisticsRequest request)
    {
        var response = _statisticsStore.Get(request.EntityType, request.Operation);
        return Ok(response);
    }
}
