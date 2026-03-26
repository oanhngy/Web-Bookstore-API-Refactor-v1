using System.ComponentModel.DataAnnotations;

namespace BookstoreWeb.Application.DTOs.Account;

public class RegisterRequest
{
    [Required]
    [EmailAddress]
    public string Email {get; set;}=string.Empty;

    [Required]
    [MinLength(6)]
    public string Password {get; set;}=string.Empty;
}