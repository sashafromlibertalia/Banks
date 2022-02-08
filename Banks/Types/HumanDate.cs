using System;
using Banks.Tools;

namespace Banks.Types
{
    public class HumanDate
    {
        private readonly int _months;
        private readonly int _years;
        public HumanDate(int months, int years)
        {
            if (months <= 0 && years <= 0)
                throw new BanksException("Can't create account for specified date range.");

            if (months < 0 || years < 0)
                throw new ArgumentNullException(string.Empty, "Invalid argument provided");

            _months = months;
            _years = years;
        }

        public DateTime Date => DateTime.Now.AddMonths(_months).AddYears(_years);
    }
}