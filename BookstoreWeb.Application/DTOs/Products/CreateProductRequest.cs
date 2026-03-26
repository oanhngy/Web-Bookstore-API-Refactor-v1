using System.ComponentModel.DataAnnotations;
namespace BookstoreWeb.Application.DTOs.Products;

public class CreateProductRequest
{
    [Required]
    [MaxLength(200)]
    public string Name {get; set;}=string.Empty;
    public string? Description {get; set;}
    public string? Author {get; set;}

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage ="Price must be greater than 0")]
    public decimal Price {get; set;}

    [Required]
    public int CategoryID {get; set;}

    //images conver from IFormFile by Controller b4 passing to Service
    public List<ProductImageData> Images {get; set;}=new();
}