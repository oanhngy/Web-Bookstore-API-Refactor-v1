using System.ComponentModel.DataAnnotations;

namespace BookstoreWeb.Application.DTOs.Categories;

// Data required from the client to create a new category
public class CreateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
