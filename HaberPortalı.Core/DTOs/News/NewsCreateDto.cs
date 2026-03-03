using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HaberPortal.Core.DTOs.News
{
    public class NewsCreateDto
    {
        [Required(ErrorMessage = "Haber başlığı gereklidir")]
        [MinLength(5, ErrorMessage = "Başlık en az 5 karakter olmalıdır")]
        [MaxLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Haber özeti gereklidir")]
        [MinLength(10, ErrorMessage = "Özet en az 10 karakter olmalıdır")]
        [MaxLength(500, ErrorMessage = "Özet en fazla 500 karakter olabilir")]
        public string Summary { get; set; } = string.Empty;

        [Required(ErrorMessage = "Haber içeriği gereklidir")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Kategori seçilmelidir")]
        public int CategoryId { get; set; }

        public bool IsPublished { get; set; } = false;

        // Etiketler
        public List<string> Tags { get; set; } = new();

        // Görseller (base64 veya url olarak)
        public List<NewsImageCreateDto> Images { get; set; } = new();
    }

    public class NewsImageCreateDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; }
    }
}