using System;
using System.Collections.Generic;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Entities
{
    public class Debit : IAccount
    {
        private readonly List<double> _addedPercents;
        public Debit(double percentage)
        {
            if (percentage < 0)
                throw new BanksException("Can't create account with negative percentage.");

            Id = Guid.NewGuid();
            Balance = 0;
            Type = AccountTypes.Debit;
            Percentage = percentage;
            _addedPercents = new List<double>();
        }

        public IReadOnlyList<double> AddedPercents => _addedPercents;
        public string Type { get; }
        public bool IsSuspicious { get; private set; }
        public double Percentage { get; }
        public double Balance { get; private set; }
        public Guid Id { get; }

        public bool IsTransferable()
        {
            return true;
        }

        public void UpdateBalance(double funds)
        {
            if (funds < 0)
                throw new BanksException("Can't add negative amount of funds.");

            Balance += funds;
        }

        public void WithDrawFunds(double funds)
        {
            if (funds < 0)
                throw new BanksException("Can't withdraw negative amount of funds.");

            if (Balance - funds < 0)
                throw new BanksException("Can't withdraw specified amount of funds. Client doesn't have enough money.");

            Balance -= funds;
        }

        public void SetSuspiciousStatus(bool status)
        {
            IsSuspicious = status;
        }

        public void AddPercents()
        {
            _addedPercents.Add(Balance * Percentage / 365 / 100);
        }
    }
}