using System;
using Newtonsoft.Json;

namespace MyFinance.Domain.Utils
{
    public class CommandResult
    {
        public int Rows { get; set; }
        public dynamic ResultData { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public string ErrorName { get { return Convert.ToString(ErrorCode); } }
        public string ErrorMessage { get; set; }
        public string Time { get; set; }

        [JsonConstructor]
        public CommandResult() { }
        public CommandResult(int rows) : this(rows, ErrorCode.None, null) { }
        public CommandResult(int rows, dynamic resultData) : this(rows, ErrorCode.None, null, resultData as object) { }
        public CommandResult(ErrorCode errorCode, string errorMessage = null) : this(0, errorCode, errorMessage) { }
        public CommandResult(int rows, ErrorCode errorCode, string errorMessage = null, dynamic resultData = null)
        {
            Rows = rows;
            ResultData = resultData;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public bool ShouldSerializeErrorCode()
        {
            return ErrorCode != ErrorCode.None;
        }

        public bool ShouldSerializeErrorName()
        {
            return ErrorCode != ErrorCode.None;
        }

        public bool ShouldSerializeResultData()
        {
            return ErrorCode == ErrorCode.None && ResultData != null;
        }

        public bool ShouldSerializeTime()
        {
            return Time != null;
        }
    }

    public enum ErrorCode
    {
        None = 0,
        OnlySysAdminAccess = 1,
        NotFound = 2,
        InvalidParameters = 3,
        NotAllowedCommand = 4,
        Unauthorized = 5,
        DuplicateUniqueIdentifier = 6,
    }
}