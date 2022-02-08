using Banks.Entities;
using Banks.Services;
using Banks.Tools;

namespace Banks.Accounts
{
    public class DebitCreator : IAccountCreator
    {
        private const double MaxPercents = 10;
        private readonly double _percentage;

        public DebitCreator(double percentage)
        {
            if (percentage < 0 && percentage > MaxPercents)
                throw new BanksException($"Can't create account with {percentage}%");

            _percentage = percentage;
        }

        public IAccount CreateAccount()
        {
            return new Debit(_percentage);
        }
    }
}