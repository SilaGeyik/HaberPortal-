namespace HaberPortal.Core.Entities
{
    public class NewsTag
    {
        public int NewsId { get; set; }
        public int TagId { get; set; }

        // Navigation properties
        public virtual News News { get; set; }
        public virtual Tag Tag { get; set; }
    }
}