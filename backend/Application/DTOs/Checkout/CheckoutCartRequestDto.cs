using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Checkout;

public class CheckoutCartRequestDto
{
    [Required]
    [MinLength(1, ErrorMessage = "O carrinho deve ter pelo menos um item.")]
    public List<CheckoutItemDto> Items { get; set; } = new();
}
