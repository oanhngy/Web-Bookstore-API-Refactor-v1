namespace BookstoreWeb.Application.DTOs.Products;

//Image upload data passed from Controller to Service
//Controller read IFormFile, convert to this DTO, Service never see IFormFile directly
public class ProductImageData
{
    public byte[] Data {get; set;}=Array.Empty<byte>();
    public string FileName {get; set;}=string.Empty;
    public bool IsPrimary {get; set;}
}