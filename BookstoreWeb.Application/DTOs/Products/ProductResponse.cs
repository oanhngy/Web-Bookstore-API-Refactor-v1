namespace BookstoreWeb.Application.DTOs.Products;

//product data response to client
public class ProductResponse
{
    public int ProductID {get; set;}
    public string Name {get; set;}=string.Empty;
    public string? Description {get; set;}
    public string? Author {get; set;}
    public decimal Price {get; set;}
    public int CategoryID {get; set;}
    public string? CategoryName {get; set;}
    public List<ProductImageResponse> Images {get; set;}=new();
}