namespace HaberPortal.Core.DTOs.News
{
    public class NewsFilterDto
    {
        public int? CategoryId { get; set; }
        public string? SearchTerm { get; set; }
        public int? AuthorId { get; set; }
        public bool? IsPublished { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Tag { get; set; }

        // Sayfalama
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sıralama
        public string SortBy { get; set; } = "PublishedAt";
        public bool SortDescending { get; set; } = true;
    }
}