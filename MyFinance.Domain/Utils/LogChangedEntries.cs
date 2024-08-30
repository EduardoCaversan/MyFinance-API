using System.Collections.Generic;

namespace MyFinance.Domain.Utils
{
    public class LogChangedEntry
    {
        public string Id { get; set; }
        public string EntityName { get; set; }
        public string EntityState { get; set; }
        public List<LogChangedProperty> Properties { get; set; }

        public LogChangedEntry()
        {
            Properties = new List<LogChangedProperty>();
        }
    }

    public class LogChangedProperty
    {
        public string PropertyName { get; set; }
        public dynamic OriginalValue { get; set; }
        public dynamic CurrentValue { get; set; }
    }
}