using System;
using System.Collections.Generic;
using System.Linq;

namespace MyFinance.Domain.Utils
{
    public class QueryResult<T> where T : class, IViewModel
    {
        public int TotalRows { get; set; }
        public T[] Data { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string ErrorName { get { return Convert.ToString(ErrorCode); } }
        public string ErrorMessage { get; set; }

        protected QueryResult() { }
        public QueryResult(T viewModel) : this(viewModel == null ? 0 : 1, viewModel == null ? new T[0] : new T[1] { viewModel }, ErrorCode.None) { }
        public QueryResult(int totalRows) : this(totalRows, new T[0], ErrorCode.None) { }
        public QueryResult(int totalRows, T[] data) : this(totalRows, data, ErrorCode.None) { }
        public QueryResult(int totalRows, IEnumerable<T> data) : this(totalRows, data.ToArray(), ErrorCode.None) { }
        public QueryResult(T[] data) : this(data?.Length ?? 0, data, ErrorCode.None) { }
        public QueryResult(ErrorCode errorCode, string errorMessage = null) : this(0, null, errorCode, errorMessage) { }
        public QueryResult(int totalRows, T[] data, ErrorCode errorCode, string errorMessage = null)
        {
            TotalRows = totalRows;
            Data = data;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }
        public QueryResult(IEnumerable<T> data)
        {
            var array = data?.ToArray() ?? new T[0];
            TotalRows = array.Length;
            Data = array;
        }

        public T FirstOrDefault()
        {
            return Data?.FirstOrDefault();
        }

        public bool ShouldSerializeErrorCode()
        {
            return ErrorCode != ErrorCode.None;
        }

        public bool ShouldSerializeErrorName()
        {
            return ErrorCode != ErrorCode.None;
        }

        public bool ShouldSerializeErrorMessage()
        {
            return ErrorMessage != null;
        }
    }
}