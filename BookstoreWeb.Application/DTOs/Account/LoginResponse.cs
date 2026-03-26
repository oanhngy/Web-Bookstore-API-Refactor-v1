namespace BookstoreWeb.Application.DTOs.Account;

//return to client after login success
public class LoginResponse
{
    public string Token {get; set;}=string.Empty; //do in phase 5
    public string Email {get; set;}=string.Empty;
    public string Role {get; set;}=string.Empty;
}