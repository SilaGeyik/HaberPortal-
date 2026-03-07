using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HaberPortalı.Core.Interfaces
{
    public interface IRepository<T> where T: class
    {
        //get işlemleri
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        //single get
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        //sayfalama
        Task<IEnumerable<T>> GetPagedAsync(int pageNumber, int pageSize);

        //add
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);

        //update
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        //remove
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        //count
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);


        //any
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
    }
}
