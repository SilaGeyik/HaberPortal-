using System.ComponentModel.DataAnnotations;

namespace UygAPI.Models
{
    public class NewsTag
    {
        [Key]
        public int NewsId { get; set; }
        [Key]
        public int TagId { get; set; }

        
        public virtual News News { get; set; }
        public virtual Tag Tag { get; set; }
    }
}