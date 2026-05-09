using System.ComponentModel.DataAnnotations;

namespace UygAPI.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public int ViewCount { get; set; } = 0;
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedAt { get; set; }

        public virtual Category Category { get; set; }
        public virtual User Author { get; set; }
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new List<Like>();
        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}