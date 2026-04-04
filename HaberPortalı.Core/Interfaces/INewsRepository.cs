using HaberPortal.Core.DTOs.News;
using HaberPortal.Core.Entities;
using HaberPortalı.Core.Interfaces;

namespace HaberPortal.Core.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {
        // News'e özel metodlar
        Task<IEnumerable<News>> GetNewsByCategoryAsync(int categoryId);
        Task<IEnumerable<News>> GetPublishedNewsAsync();
        Task<IEnumerable<News>> GetLatestNewsAsync(int count);
        Task<IEnumerable<News>> GetMostViewedNewsAsync(int count);
        Task<IEnumerable<News>> SearchNewsAsync(string searchTerm);
        Task<News> GetNewsWithDetailsAsync(int id); // Yorumlar, etiketler, görsellerle birlikte
        Task<bool> IsSlugUniqueAsync(string slug);
        Task IncrementViewCountAsync(int id);
        Task<IEnumerable<News>> GetNewsByTagAsync(string tagSlug);
        Task<PagedNewsResult> GetPagedNewsAsync(NewsFilterDto filter);
    }

    public class PagedNewsResult
    {
        public IEnumerable<NewsListDto> News { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
}