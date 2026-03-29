namespace BookstoreWeb.Application.Domain;

public class Product
{
    public int    ProductID   { get; set; }
    public string Name        { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Author      { get; set; }
    public decimal Price      { get; set; }
    public int    CategoryID  { get; set; }

    // Navigation properties
    public Category?            Category      { get; set; }
    public List<ProductImage>?  ProductImages { get; set; }
}
