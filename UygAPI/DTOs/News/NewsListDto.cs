namespace UygAPI.DTOs.News
{
    public class NewsListDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public DateTime? PublishedAt { get; set; }
    }
}