namespace UygAPI.DTOs.Comment
{
    public class CommentDetailDto
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int NewsId { get; set; }
        public string NewsTitle { get; set; } = string.Empty;
    }
}