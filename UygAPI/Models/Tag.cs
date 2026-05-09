using System.ComponentModel.DataAnnotations;

namespace UygAPI.Models
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;

        public virtual ICollection<NewsTag> NewsTags { get; set; } = new List<NewsTag>();
    }
}