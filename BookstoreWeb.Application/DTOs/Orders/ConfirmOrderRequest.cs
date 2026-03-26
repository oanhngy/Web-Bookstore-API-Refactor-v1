using System.ComponentModel.DataAnnotations;

namespace BookstoreWeb.Application.DTOs.Orders;

public class ConfirmOrderRequest
{
    [Required]
    public string FullName {get; set;}=string.Empty;
    [Required]
    public string Email {get; set;}=string.Empty;
    [Required]
    public string Phone {get; set;}=string.Empty;
    [Required]
    public string Address {get; set;}=string.Empty;
    public string? Note {get; set;}
    [Required]
    public string PaymentMethod {get; set;}=string.Empty;
}