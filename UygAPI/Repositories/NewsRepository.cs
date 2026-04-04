using Microsoft.EntityFrameworkCore;
using UygAPI.Data;
using UygAPI.Interfaces;
using UygAPI.Models;

namespace UygAPI.Repositories
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<News>> GetNewsWithCategoryAsync()
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();
        }

        public async Task<News?> GetNewsWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Include(n => n.Comments)
                    .ThenInclude(c => c.User)
                .Include(n => n.Likes)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<IEnumerable<News>> GetLatestNewsAsync(int count)
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetPopularNewsAsync(int count)
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.ViewCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetNewsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Where(n => n.CategoryId == categoryId && n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> SearchNewsAsync(string searchTerm)
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Where(n => n.IsPublished &&
                    (n.Title.Contains(searchTerm) ||
                     n.Content.Contains(searchTerm) ||
                     n.Summary.Contains(searchTerm)))
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();
        }

        public async Task<bool> IncrementViewCountAsync(int id)
        {
            var news = await GetByIdAsync(id);
            if (news == null)
                return false;

            news.ViewCount++;
            Update(news);
            return true;
        }
    }
}