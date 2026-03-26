namespace BookstoreWeb.Application.Exceptions;

//operation conflict w/ existing data --> 409
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message)
    {
        
    }
}