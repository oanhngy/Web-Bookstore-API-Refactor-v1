namespace BookstoreWeb.Application.Exceptions;

//input valid but fail business rule --> 400
public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
        
    }
}