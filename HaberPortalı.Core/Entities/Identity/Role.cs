using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace HaberPortal.Core.Entities.Identity
{
    public class Role : IdentityRole<int>
    {
        public string? Description { get; set; }

        // Navigation property - BU SATIRI EKLE
        public virtual ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    }
}