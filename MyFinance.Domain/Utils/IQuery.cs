using MyFinance.Domain.Queries;
using System.Threading.Tasks;

namespace MyFinance.Domain.Utils
{
    public interface IQuery
    {
        bool IsValid();
        Task<QueryResult<IViewModel>> ExecuteAsync(QueriesHandler queriesHandler);
        Task<bool> HasPermissionAsync(QueriesHandler queriesHandler);
    }

    public interface IQuery<T> : IQuery where T : class, IViewModel
    {
        new Task<QueryResult<T>> ExecuteAsync(QueriesHandler queriesHandler);
    }

    public abstract class AbstractQuery<T> : IQuery, IQuery<T> where T : class, IViewModel
    {
        public abstract bool IsValid();
        public abstract Task<bool> HasPermissionAsync(QueriesHandler queriesHandler);
        public abstract Task<QueryResult<T>> ExecuteAsync(QueriesHandler queriesHandler);
        async Task<QueryResult<IViewModel>> IQuery.ExecuteAsync(QueriesHandler queriesHandler)
        {
            return await ExecuteAsync(queriesHandler) as QueryResult<IViewModel>;
        }
    }
}