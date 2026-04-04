using System.ComponentModel.DataAnnotations;

namespace UygAPI.DTOs.Category
{
    public class CategoryUpdateDto
    {
        [Required(ErrorMessage = "Kategori adı zorunludur")]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
    }
}