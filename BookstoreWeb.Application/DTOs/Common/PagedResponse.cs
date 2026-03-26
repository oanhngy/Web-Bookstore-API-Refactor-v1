namespace BookstoreWeb.Application.DTOs.Common;

//generic wrapper cho paginated API response
public class PagedResponse<T>
{
    public IEnumerable<T> Items {get; set;}=new List<T>();
    public int Page {get; set;}
    public int PageSize {get; set;}
    public int TotalCount {get; set;}

    //auto
    public int TotalPages => (int)Math.Ceiling((double)TotalCount/PageSize);
}