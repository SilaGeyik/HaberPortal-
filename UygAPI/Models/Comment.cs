using System.ComponentModel.DataAnnotations;

namespace UygAPI.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int NewsId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsApproved { get; set; } = true;

        public virtual News News { get; set; }
        public virtual User User { get; set; }
    }
}