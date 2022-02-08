using Banks.Types;

namespace Banks.Services
{
    public interface INotifier
    {
        BankData State { get; }
        void AddSubscriber(ISubscriber subscriber);
        void RemoveSubscriber(ISubscriber subscriber);
        void Notify(string accountType);
    }
}