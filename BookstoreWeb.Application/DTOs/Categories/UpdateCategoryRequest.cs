using System.ComponentModel.DataAnnotations;

namespace BookstoreWeb.Application.DTOs.Categories;

// Data required from the client to update an existing category
public class UpdateCategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
}
