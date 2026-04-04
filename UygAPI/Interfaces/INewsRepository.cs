using UygAPI.Models;

namespace UygAPI.Interfaces
{
    public interface INewsRepository : IRepository<News>
    {
        Task<IEnumerable<News>> GetNewsWithCategoryAsync();
        Task<News?> GetNewsWithDetailsAsync(int id);
        Task<IEnumerable<News>> GetLatestNewsAsync(int count);
        Task<IEnumerable<News>> GetPopularNewsAsync(int count);
        Task<IEnumerable<News>> GetNewsByCategoryAsync(int categoryId);
        Task<IEnumerable<News>> SearchNewsAsync(string searchTerm);
        Task<bool> IncrementViewCountAsync(int id);
    }
}