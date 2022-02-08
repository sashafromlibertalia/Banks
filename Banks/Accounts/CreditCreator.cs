using Banks.Entities;
using Banks.Services;
using Banks.Tools;

namespace Banks.Accounts
{
    public class CreditCreator : IAccountCreator
    {
        private readonly double _limit;
        private readonly double _commission;

        public CreditCreator(double limit, double commission)
        {
            if (limit < 0 || commission < 0)
                throw new BanksException("Can't create this account with specified arguments.");

            _limit = limit;
            _commission = commission;
        }

        public IAccount CreateAccount()
        {
            return new Credit(_limit, _commission);
        }
    }
}