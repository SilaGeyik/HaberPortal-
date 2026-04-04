using HaberPortal.Core.Entities.Base;
using HaberPortal.Core.Entities.Identity;

namespace HaberPortal.Core.Entities
{
    public class Like : BaseEntity
    {
        // Foreign keys
        public int NewsId { get; set; }
        public int UserId { get; set; }

        // Navigation properties
        public virtual News News { get; set; }
        public virtual User User { get; set; }
    }
}