using UygAPI.Data;
using UygAPI.Interfaces;
using UygAPI.Models;

namespace UygAPI.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        private IRepository<Category>? _categories;
        private INewsRepository? _news;
        private IRepository<Comment>? _comments;
        private IRepository<Like>? _likes;
        private IRepository<Tag>? _tags;
        private IRepository<NewsTag>? _newsTags;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IRepository<Category> Categories =>
            _categories ??= new Repository<Category>(_context);

        public INewsRepository News =>
            _news ??= new NewsRepository(_context);

        public IRepository<Comment> Comments =>
            _comments ??= new Repository<Comment>(_context);

        public IRepository<Like> Likes =>
            _likes ??= new Repository<Like>(_context);

        public IRepository<Tag> Tags =>
            _tags ??= new Repository<Tag>(_context);

        public IRepository<NewsTag> NewsTags =>
            _newsTags ??= new Repository<NewsTag>(_context);

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}