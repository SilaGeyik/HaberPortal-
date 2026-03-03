using HaberPortal.Core.Entities.Base;
using HaberPortal.Core.Entities;
using System.Collections.Generic;

namespace HaberPortal.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Slug { get; set; }

        
        public virtual ICollection<News> News { get; set; }
    }
}