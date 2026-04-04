using System.ComponentModel.DataAnnotations;

namespace UygAPI.DTOs.Category
{
    public class CategoryCreateDto
    {
        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}