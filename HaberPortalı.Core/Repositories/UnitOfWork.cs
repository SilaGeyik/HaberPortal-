using HaberPortal.Core.Data;
using HaberPortal.Core.Entities;
using HaberPortal.Core.Interfaces;
using HaberPortalı.Core.Interfaces;

namespace HaberPortal.Core.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private INewsRepository _newsRepository;
        private IRepository<Category> _categoryRepository;
        private IRepository<Comment> _commentRepository;
        private IRepository<Tag> _tagRepository;
        private IRepository<Like> _likeRepository;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public INewsRepository News =>
            _newsRepository ??= new NewsRepository(_context);

        public IRepository<Category> Categories =>
            _categoryRepository ??= new Repository<Category>(_context);

        public IRepository<Comment> Comments =>
            _commentRepository ??= new Repository<Comment>(_context);

        public IRepository<Tag> Tags =>
            _tagRepository ??= new Repository<Tag>(_context);

        public IRepository<Like> Likes =>
            _likeRepository ??= new Repository<Like>(_context);

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