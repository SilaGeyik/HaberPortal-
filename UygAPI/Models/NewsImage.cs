using System.ComponentModel.DataAnnotations;
using UygAPI.Models.Identity;

namespace UygAPI.Models
{
    public class NewsImage : BaseEntity
    {
        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; }

        [MaxLength(200)]
        public string Caption { get; set; }

        public int Order { get; set; } = 0;

        // Foreign Keys
        public int NewsId { get; set; }

        // Navigation Properties
        public virtual News News { get; set; }
    }
}