using System.ComponentModel.DataAnnotations;

namespace UygAPI.DTOs.News
{
    public class NewsCreateDto
    {
        [Required(ErrorMessage = "Haber başlığı zorunludur")]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Haber içeriği zorunludur")]
        public string Content { get; set; } = string.Empty;

        [MaxLength(500)]
        public string Summary { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}