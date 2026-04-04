using System.ComponentModel.DataAnnotations;

namespace UygAPI.Models
{
    public class Like
    {
        [Key]
        public int Id { get; set; }
        public int NewsId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public virtual News News { get; set; }
        public virtual User User { get; set; }
    }
}