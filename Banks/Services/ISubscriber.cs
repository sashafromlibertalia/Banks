using System.Collections.Generic;

namespace Banks.Services
{
    public interface ISubscriber
    {
        IReadOnlyList<IAccount> Accounts { get; }
        void FetchUpdates();
    }
}