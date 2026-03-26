namespace BookstoreWeb.Application.DTOs.Categories;

// Data returned to the client when reading a category
public class CategoryResponse
{
    public int    CategoryID { get; set; }
    public string Name       { get; set; } = string.Empty;
}
