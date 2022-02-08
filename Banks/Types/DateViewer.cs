using System;

namespace Banks.Types
{
    public class DateViewer
    {
        public DateViewer()
        {
        }

        public DateTime Date { get; private set; }

        public void OnDate(int days, int months, int years)
        {
            if (days < 0 || months < 0 || years < 0)
                throw new ArgumentException("Wrong date limits added.");

            Date = DateTime.Now.AddDays(days).AddMonths(months).AddYears(years);
        }

        public void OneDay()
        {
            Date = DateTime.Now.AddDays(1);
        }

        public void OneMonth()
        {
            Date = DateTime.Now.AddMonths(1);
        }

        public void OneYear()
        {
            Date = DateTime.Now.AddYears(1);
        }
    }
}