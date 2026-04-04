using UygAPI.DTOs.Comment;

namespace UygAPI.DTOs.News
{
    public class NewsDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public List<CommentListDto> Comments { get; set; } = new List<CommentListDto>();
    }
}