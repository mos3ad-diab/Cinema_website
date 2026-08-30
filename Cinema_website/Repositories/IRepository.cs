using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema_website.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<EntityEntry<T>> InsertAsync(T entity);


        void Update(T entity);

        void Delete(T entity);



        IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            );


        Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            string contain = "",
            bool isTracked = true
            );



        Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,

            bool isTracked = true
            );


        Task<int> CommitAsync();
        
    }
}

