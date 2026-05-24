using Backend.Application.DTOs.Checkout;
using Backend.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Presentation.Controllers;

[ApiController]
[Route("checkout")]
public class CheckoutController : ControllerBase
{
    private readonly ICheckoutService _checkoutService;
    private readonly IIdempotencyStore _idempotencyStore;
    private readonly ILogger<CheckoutController> _logger;

    public CheckoutController(
        ICheckoutService checkoutService,
        IIdempotencyStore idempotencyStore,
        ILogger<CheckoutController> logger)
    {
        _checkoutService = checkoutService;
        _idempotencyStore = idempotencyStore;
        _logger = logger;
    }

    /// <summary>Realiza o checkout de um carrinho com múltiplos itens.</summary>
    /// <remarks>Suporta idempotência via header <c>Idempotency-Key</c>.</remarks>
    /// <response code="200">Checkout realizado com sucesso.</response>
    /// <response code="400">Carrinho vazio ou dados inválidos.</response>
    /// <response code="404">Produto não encontrado.</response>
    /// <response code="422">Estoque insuficiente.</response>
    [HttpPost("cart")]
    public IActionResult CheckoutCart(CheckoutCartRequestDto request)
    {
        var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();

        if (!string.IsNullOrWhiteSpace(idempotencyKey) &&
            _idempotencyStore.TryGet(idempotencyKey, out var cachedResult))
        {
            _logger.LogInformation("[{TraceId}] Idempotency-Key={Key} already processed, returning cached result",
                HttpContext.TraceIdentifier, idempotencyKey);
            return BuildResponse(cachedResult);
        }

        _logger.LogInformation("[{TraceId}] Cart checkout requested for {ItemCount} item(s)",
            HttpContext.TraceIdentifier, request.Items?.Count ?? 0);

        var result = _checkoutService.ProcessCheckout(request.Items ?? new List<CheckoutItemDto>());

        _logger.LogInformation("[{TraceId}] Cart checkout result: {Result}",
            HttpContext.TraceIdentifier, result);

        if (!string.IsNullOrWhiteSpace(idempotencyKey))
            _idempotencyStore.Set(idempotencyKey, result);

        return BuildResponse(result);
    }

    private IActionResult BuildResponse(CheckoutResult result)
    {
        if (result == CheckoutResult.ProductNotFound)
            return NotFound(new { message = "Produto não encontrado." });

        if (result == CheckoutResult.InsufficientStock)
            return UnprocessableEntity(new { message = "Estoque insuficiente." });

        return Ok(new { message = "Compra realizada com sucesso." });
    }
}
