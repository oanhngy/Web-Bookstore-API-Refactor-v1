using System.ComponentModel.DataAnnotations;

namespace BookstoreWeb.Application.DTOs.Cart;
 public class AddToCartRequest
{
    [Required]
    public int ProductId {get; set;}

    [Required]
    [Range(1, int.MaxValue, ErrorMessage ="Quantity must be at leasr 1")]
    public int Quantity {get; set;}
}