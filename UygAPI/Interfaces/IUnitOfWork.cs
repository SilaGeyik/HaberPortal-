using UygAPI.Models;

namespace UygAPI.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Category> Categories { get; }
        INewsRepository News { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Like> Likes { get; }
        IRepository<Tag> Tags { get; }
        IRepository<NewsTag> NewsTags { get; }

        Task<int> CompleteAsync();
    }
}