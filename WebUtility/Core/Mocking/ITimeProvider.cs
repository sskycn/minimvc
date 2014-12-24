using System;

namespace Cvv.WebUtility.Core
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}