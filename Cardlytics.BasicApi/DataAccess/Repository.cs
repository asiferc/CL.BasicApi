using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Cardlytics.BasicApi.Models;

using Microsoft.EntityFrameworkCore;

namespace Cardlytics.BasicApi.DataAccess
{
    public class Repository<T> : IRepository<T>
        where T : BaseEntity, new()
    {
        private readonly DbSet<T> _set;

        public Repository(BasicDbContext context)
        {
            Context = context;
            _set = context.Set<T>();
        }

        public DbContext Context { get; }

        public async Task<T> GetByIdAsync(int id, BaseFilter baseFilter)
        {
            var query = (IQueryable<T>)_set;

            if (baseFilter != null && baseFilter.Include.Any())
            {
                query = GetIncludedObjects(query, baseFilter);
            }

            return await query.FirstOrDefaultAsync(i => i.Id == id);
        }

        public Task<List<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            BaseFilter baseFilter = null)
        {
            var query = orderBy == null ? (IQueryable<T>)_set : orderBy(_set);

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (baseFilter != null)
            {
                query = GetIncludedObjects(query, baseFilter);

                if (baseFilter.PageNumber > 1)
                {
                    query = query.Skip((baseFilter.PageNumber - 1) * baseFilter.PageSize);
                }

                query = query.Take(baseFilter.PageSize);
            }

            return query.ToListAsync();
        }

        protected virtual Expression<Func<T, object>> GetIncludedObjects(string tableName)
        {
            switch (tableName.ToLowerInvariant())
            {
                // Any case statements for related data go here
                default:
                    return null;
            }
        }

        private IQueryable<T> GetIncludedObjects(IQueryable<T> baseQuery, BaseFilter baseFilter)
        {
            var query = baseQuery;

            if (baseFilter != null && baseFilter.Include.Any())
            {
                foreach (var includeTableName in baseFilter.Include)
                {
                    var includeExpression = GetIncludedObjects(includeTableName);

                    if (includeExpression != null)
                    {
                        query = query.Include(includeExpression);
                    }
                }
            }

            return query;
        }
    }
}