using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HaberPortal.Core.Entities.Identity
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<News> News { get; set; } = new HashSet<News>();
        public virtual ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
        public virtual ICollection<Like> Likes { get; set; } = new HashSet<Like>();
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>(); // BU SATIRI EKLE
    }
}