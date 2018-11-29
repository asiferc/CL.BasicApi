using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Cardlytics.BasicApi.Models;

using Microsoft.EntityFrameworkCore;

namespace Cardlytics.BasicApi.DataAccess
{
    public interface IRepository<T>
    {
        DbContext Context { get; }

        Task<T> GetByIdAsync(int id, BaseFilter baseFilter);

        Task<List<T>> GetAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            BaseFilter baseFilter = null);
    }
}
