using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Accounts
{
    public class DepositCreator : IAccountCreator
    {
        private readonly HumanDate _date;
        private readonly double _money;
        private readonly double _percentage;
        public DepositCreator(HumanDate date, double money, double percentage)
        {
            if (percentage < 0 || money < 0)
                throw new BanksException("Can't create account with specified arguments.");

            _date = date;
            _money = money;
            _percentage = percentage;
        }

        public IAccount CreateAccount()
        {
            return new Deposit(_date, _money, _percentage);
        }
    }
}