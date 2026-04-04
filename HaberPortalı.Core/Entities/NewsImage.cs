using HaberPortal.Core.Entities.Base;

namespace HaberPortal.Core.Entities
{
    public class NewsImage : BaseEntity
    {
        public string ImageUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsDefault { get; set; } = false;

        // Foreign keys
        public int NewsId { get; set; }

        // Navigation property
        public virtual News News { get; set; }
    }
}