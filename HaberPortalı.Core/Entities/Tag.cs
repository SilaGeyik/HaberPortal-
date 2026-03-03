using HaberPortal.Core.Entities.Base;
using HaberPortal.Core.Entities;
using System.Collections.Generic;

namespace HaberPortal.Core.Entities
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public string Slug { get; set; }

        // Navigation property
        public virtual ICollection<NewsTag> NewsTags { get; set; }
    }
}
