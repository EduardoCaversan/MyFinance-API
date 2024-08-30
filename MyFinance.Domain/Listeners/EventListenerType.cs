using System;

namespace MyFinance.Domain.Events
{
    [Flags]
    public enum EventListenerType
    {
        None = 0,
        All = 1
    }
}
