using System.ComponentModel.DataAnnotations;

namespace BookstoreWeb.Infrastructure.Data;

public class ProductImage
{
    [Key]
    public int     ImageID   { get; set; }
    public int     ProductID { get; set; }
    public string  ImagePath { get; set; } = string.Empty;
    public bool    IsPrimary { get; set; }
    public string  ImageType { get; set; } = string.Empty;

    // Navigation property
    public Product? Product  { get; set; }
}
