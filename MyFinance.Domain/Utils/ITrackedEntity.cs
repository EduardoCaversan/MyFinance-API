using System;

namespace MyFinance.Domain.Utils
{
    public interface ITrackedEntity
    {
        DateTimeOffset? LastModified { get; set; }
        bool IsOfflineCommand { get; set; }
    }
}