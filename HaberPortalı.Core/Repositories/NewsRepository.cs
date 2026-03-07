using Microsoft.EntityFrameworkCore;
using HaberPortal.Core.Data;
using HaberPortal.Core.Entities;
using HaberPortal.Core.Interfaces;
using HaberPortal.Core.DTOs.News;

namespace HaberPortal.Core.Repositories
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<News>> GetNewsByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(n => n.CategoryId == categoryId && n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetPublishedNewsAsync()
        {
            return await _dbSet
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetLatestNewsAsync(int count)
        {
            return await _dbSet
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.PublishedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> GetMostViewedNewsAsync(int count)
        {
            return await _dbSet
                .Where(n => n.IsPublished)
                .OrderByDescending(n => n.ViewCount)
                .Take(count)
                .ToListAsync();
        }

        public async Task<IEnumerable<News>> SearchNewsAsync(string searchTerm)
        {
            return await _dbSet
                .Where(n => n.IsPublished &&
                           (n.Title.Contains(searchTerm) ||
                            n.Summary.Contains(searchTerm) ||
                            n.Content.Contains(searchTerm)))
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();
        }

        public async Task<News> GetNewsWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(n => n.Category)
                .Include(n => n.Author)
                .Include(n => n.Images)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Include(n => n.Comments)
                    .ThenInclude(c => c.User)
                .Include(n => n.Likes)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<bool> IsSlugUniqueAsync(string slug)
        {
            return !await _dbSet.AnyAsync(n => n.Slug == slug);
        }

        public async Task IncrementViewCountAsync(int id)
        {
            var news = await _dbSet.FindAsync(id);
            if (news != null)
            {
                news.ViewCount++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<News>> GetNewsByTagAsync(string tagSlug)
        {
            return await _dbSet
                .Where(n => n.IsPublished &&
                           n.NewsTags.Any(nt => nt.Tag.Slug == tagSlug))
                .OrderByDescending(n => n.PublishedAt)
                .ToListAsync();
        }

        public async Task<PagedNewsResult> GetPagedNewsAsync(NewsFilterDto filter)
        {
            var query = _dbSet.Where(n => n.IsPublished).AsQueryable();

            // Filtreleme
            if (filter.CategoryId.HasValue)
                query = query.Where(n => n.CategoryId == filter.CategoryId);

            if (!string.IsNullOrEmpty(filter.SearchTerm))
                query = query.Where(n => n.Title.Contains(filter.SearchTerm) ||
                                         n.Summary.Contains(filter.SearchTerm));

            if (filter.StartDate.HasValue)
                query = query.Where(n => n.PublishedAt >= filter.StartDate);

            if (filter.EndDate.HasValue)
                query = query.Where(n => n.PublishedAt <= filter.EndDate);

            // Sıralama
            query = filter.SortBy switch
            {
                "Title" => filter.SortDescending
                    ? query.OrderByDescending(n => n.Title)
                    : query.OrderBy(n => n.Title),
                "ViewCount" => filter.SortDescending
                    ? query.OrderByDescending(n => n.ViewCount)
                    : query.OrderBy(n => n.ViewCount),
                _ => filter.SortDescending
                    ? query.OrderByDescending(n => n.PublishedAt)
                    : query.OrderBy(n => n.PublishedAt)
            };

            // Toplam sayı
            var totalCount = await query.CountAsync();

            // Sayfalama
            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(n => new NewsListDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Summary = n.Summary,
                    Slug = n.Slug,
                    ImageUrl = n.Images.FirstOrDefault(i => i.IsDefault).ImageUrl ?? n.Images.FirstOrDefault().ImageUrl,
                    ViewCount = n.ViewCount,
                    CommentCount = n.Comments.Count(c => c.IsApproved),
                    LikeCount = n.Likes.Count,
                    CategoryName = n.Category.Name,
                    AuthorName = n.Author.FirstName + " " + n.Author.LastName,
                    PublishedAt = n.PublishedAt.Value,
                    IsPublished = n.IsPublished
                })
                .ToListAsync();

            return new PagedNewsResult
            {
                News = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }
    }
}