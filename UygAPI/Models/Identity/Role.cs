using Microsoft.AspNetCore.Identity;

namespace UygAPI.Models
{
    public class Role : IdentityRole<int>
    {
        public string Description { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}