using System.ComponentModel.DataAnnotations;

namespace UygAPI.DTOs.Comment
{
    public class CommentUpdateDto
    {
        [Required(ErrorMessage = "Yorum içeriği zorunludur")]
        [MinLength(2)]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;
    }
}