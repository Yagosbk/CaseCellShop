using System.ComponentModel.DataAnnotations;

namespace Backend.Application.DTOs.Checkout;

public class CheckoutItemDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "O campo ProductId deve ser maior que 0.")]
    public int ProductId { get; set; }

    [Required]
    [Range(1, 100, ErrorMessage = "A quantidade deve estar entre 1 e 100.")]
    public int Quantity { get; set; }
}
