namespace BookstoreWeb.Application.Exceptions;

//throw when no exist --> HTTP 404
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
        
    }
}