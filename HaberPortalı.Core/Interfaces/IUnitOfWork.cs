using HaberPortal.Core.Entities;
using HaberPortalı.Core.Interfaces;

namespace HaberPortal.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        INewsRepository News { get; }
        IRepository<Category> Categories { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Tag> Tags { get; }
        IRepository<Like> Likes { get; }

        Task<int> CompleteAsync();
    }
}