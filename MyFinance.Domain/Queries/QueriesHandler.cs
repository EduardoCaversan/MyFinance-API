using System.Collections.Concurrent;
using System.Reflection;
using MyFinance.Domain.Utils;
using Newtonsoft.Json.Linq;

namespace MyFinance.Domain.Queries
{
    public class QueriesHandler
    {
        public RequestData CurrentRequest { get; set; }
        public QueriesDbContext QueriesDbContext { get; private set; }

        public QueriesHandler(QueriesDbContext queriesDbContext)
        {
            QueriesDbContext = queriesDbContext;
        }

        public async Task<QueryResult<T>> RunQuery<T>(IQuery<T> query) where T : class, IViewModel
        {
            if (!query.IsValid())
                return new QueryResult<T>(ErrorCode.InvalidParameters, "Um ou mais parâmetros estão inválidos.");
            var hasPermission = await query.HasPermissionAsync(this);
            if (!hasPermission)
                return new QueryResult<T>(ErrorCode.Unauthorized, "Você não esta autorizado executar essa consulta.");
            return await query.ExecuteAsync(this);
        }

        public async Task<QueryResult<IViewModel>> RunQuery(IQuery query)
        {
            if (!query.IsValid())
                return new QueryResult<IViewModel>(ErrorCode.InvalidParameters, "Um ou mais parâmetros estão inválidos.");
            var hasPermission = await query.HasPermissionAsync(this);
            if (!hasPermission)
                return new QueryResult<IViewModel>(ErrorCode.Unauthorized, "Você não esta autorizado executar essa consulta.");
            return await query.ExecuteAsync(this);
        }

        public async Task<List<QueryResult<IViewModel>>> RunQuery(params IQuery[] queries)
        {
            var lst = new List<QueryResult<IViewModel>>();
            foreach (var q in queries)
                lst.Add(await RunQuery(q));
            return lst;
        }

        public async Task<List<QueryResult<IViewModel>>> RunQuery(QueryData[] queries)
        {
            var lst = new List<IQuery>();
            foreach (var c in queries)
            {
                var queryName = c.Query;
                var cmd = GetQuery(queryName, c.Data as JObject);
                lst.Add(cmd);
            }
            return await RunQuery(lst.ToArray());
        }

        private static readonly Lazy<ConcurrentDictionary<string, Type>> queryTypesLazy =
            new Lazy<ConcurrentDictionary<string, Type>>(GetQueryTypes);

        private static ConcurrentDictionary<string, Type> QueryTypes => queryTypesLazy.Value;

        private static ConcurrentDictionary<string, Type> GetQueryTypes()
        {
            var type = typeof(IQuery);
            var lstTypes = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t != type && type.IsAssignableFrom(t))
                .ToArray();

            var lst = new ConcurrentDictionary<string, Type>();
            foreach (var t in lstTypes)
            {
                lst[t.Name] = t;
            }

            return lst;
        }

        public IQuery GetQuery(string queryName, JObject query)
        {
            if (string.IsNullOrEmpty(queryName) || !QueryTypes.ContainsKey(queryName))
                return null;

            var type = QueryTypes[queryName];
            if (type == null)
                return null;

            return (query ?? new JObject()).ToObject(type) as IQuery;
        }
    }
}