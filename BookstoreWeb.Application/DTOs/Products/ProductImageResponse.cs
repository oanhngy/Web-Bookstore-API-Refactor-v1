namespace BookstoreWeb.Application.DTOs.Products;

public class ProductImageResponse
{
    public int ImageID {get; set;}
    public string ImagePath {get; set;}=string.Empty;
    public bool IsPrimary {get; set;}
}