using System.ComponentModel.DataAnnotations;

namespace BookstoreWeb.Application.DTOs.Orders;

public class UpdateOrderStatusRequest
{
    [Required]
    public string Status {get; set;}=string.Empty;
}