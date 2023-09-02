using IIS_Manager.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace IIS_Manager.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        internal DbSet<T> DbSet;

        public Repository(Microsoft.EntityFrameworkCore.DbContext webDbContext)
        {
            DbSet = webDbContext.Set<T>();
        }

        public T? GetSingleOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (includeProperties != null)
            {
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
            query = query.Where(filter);

            return query.SingleOrDefault();
        }

        public T? GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (includeProperties != null)
            {
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
            query = query.Where(filter);

            return query.FirstOrDefault();
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (filter is not null) query = query.Where(filter);
            if (includeProperties != null)
            {
                query = includeProperties.Split(new[] { ',' }, StringSplitOptions.TrimEntries | StringSplitOptions.TrimEntries)
                    .Aggregate(query, (current, includeProperty) => current.Include(includeProperty));
            }
            return query.ToList();
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}
