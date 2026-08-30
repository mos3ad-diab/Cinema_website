using Cinema_website.Data;
using Cinema_website.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace Cinema_website.Repositories
{
    public class Repository<T>: IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context = new ApplicationDbContext();
        private readonly DbSet<T> _dbSet;

        public Repository()
        {
            _dbSet = _context.Set<T>();
        }
        public async Task<EntityEntry<T>> InsertAsync(T entity)
        {
            return await _dbSet.AddAsync(entity);
        }
        public void Update(T entity)
        {
             _dbSet.Update(entity);
        }
        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }


        public IQueryable<T> Query(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            bool isTracked = true
            )
        {
            var entities = _dbSet.AsQueryable();
            if (filter != null)
            {
                entities = entities.Where(filter);
            }
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    entities = entities.Include(include);
                }
            }
            if (!isTracked)
            {
                entities = entities.AsNoTracking();
            }
            
            return  entities;
        }
        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T,bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            string contain = "",
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes,isTracked);
            return await entities.ToListAsync();
        }


        public async Task<T> GetOneAsync(
            Expression<Func<T, bool>>? filter = null,
            Expression<Func<T, object>>[]? includes = null,
            
            bool isTracked = true
            )
        {
            var entities = Query(filter, includes, isTracked);
            return await entities.FirstOrDefaultAsync();
        }

        public async Task<int> CommitAsync()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return -1;
            }
        }
    }
}
