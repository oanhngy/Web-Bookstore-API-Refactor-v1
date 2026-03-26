using BookstoreWeb.Application.DTOs.Account;

public interface IAccountService
{
    //register acc w Customer role, throw ConfictException
    Task RegisterAsync(RegisterRequest request);

    //validate credential + return login data, throw ValidationExcp
    Task<LoginResponse> LoginAsync(LoginRequest request);
}