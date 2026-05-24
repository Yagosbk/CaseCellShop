using Backend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _service;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IProductService service, ILogger<ProductsController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>Lista todos os produtos disponíveis.</summary>
    /// <response code="200">Retorna a lista de produtos.</response>
    [HttpGet]
    public IActionResult Get()
    {
        _logger.LogInformation("[{TraceId}] GET /products received", HttpContext.TraceIdentifier);

        var products = _service.GetProducts();

        _logger.LogInformation("[{TraceId}] Returned {Count} products", HttpContext.TraceIdentifier, products.Count);

        return Ok(products);
    }
}
