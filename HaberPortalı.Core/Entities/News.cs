using HaberPortal.Core.Entities.Base;
using HaberPortal.Core.Entities.Identity;
using HaberPortal.Core.Entities;
using System;
using System.Collections.Generic;

namespace HaberPortal.Core.Entities
{
    public class News : BaseEntity
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string Slug { get; set; }
        public int ViewCount { get; set; } = 0;
        public bool IsPublished { get; set; } = false;
        public DateTime? PublishedAt { get; set; }

        // Foreign keys
        public int CategoryId { get; set; }
        public int AuthorId { get; set; }
        public int? DefaultImageId { get; set; }

        // Navigation properties
        public virtual Category Category { get; set; }
        public virtual User Author { get; set; }
        public virtual NewsImage DefaultImage { get; set; }
        public virtual ICollection<NewsImage> Images { get; set; }
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<NewsTag> NewsTags { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
    }
}