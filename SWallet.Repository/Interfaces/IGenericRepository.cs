using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using SWallet.Domain.Paginate;
using SWallet.Repository.Implement;

namespace SWallet.Repository.Interfaces
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        #region Get Async

        Task<T> SingleOrDefaultAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<TResult> SingleOrDefaultAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<ICollection<T>> GetListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);

        Task<ICollection<TResult>> GetListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null);
        Task<ICollection<T>> GetListWithTakeAsync(
                            Expression<Func<T, bool>> predicate = null,
                            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
                            int? take = null,
                            int skip = 0);
        Task<IPaginate<T>> GetPagingListAsync(
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int page = 1,
            int size = 10);

        Task<IPaginate<TResult>> GetPagingListAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int page = 1,
            int size = 10);
        Task<IPaginate<TResult>> GetPagingListAsyncWithDistinct<TResult>(
            Expression<Func<T, TResult>> selector,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
            int page = 1,
            int size = 10,
            bool distinct = false);
        Task<IPaginate<TResult>> GetGroupedPagingListAsync<TGroupKey, TResult>(
    Expression<Func<T, TGroupKey>> groupByKey,
    Expression<Func<IGrouping<TGroupKey, T>, TResult>> groupSelector,
    Expression<Func<T, bool>> predicate = null,
    Func<IQueryable<IGrouping<TGroupKey, T>>, IOrderedQueryable<IGrouping<TGroupKey, T>>> orderBy = null,
    Func<IQueryable<T>, IIncludableQueryable<T, object>> include = null,
    int page = 1,
    int size = 10);
        #endregion

        #region Insert

        Task InsertAsync(T entity);

        Task InsertRangeAsync(IEnumerable<T> entities);

        #endregion

        #region Update

        void UpdateAsync(T entity);

        void UpdateRange(IEnumerable<T> entities);

        #endregion

        void DeleteAsync(T entity);
        void DeleteRangeAsync(IEnumerable<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetQueryable();
    }
}
