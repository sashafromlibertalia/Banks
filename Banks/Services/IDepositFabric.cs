using Banks.Types;

namespace Banks.Services
{
    public interface IDepositFabric
    {
        void UseFabric(HumanDate humanDate, double money, double percentage);
    }
}