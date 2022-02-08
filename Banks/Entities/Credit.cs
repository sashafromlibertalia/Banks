using System;
using System.Collections.Generic;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Entities
{
    public class Credit : IAccount
    {
        public Credit(double limit, double commission)
        {
            if (limit < 0 || commission < 0)
                throw new BanksException("Can't create this account with specified arguments.");

            Id = Guid.NewGuid();
            Balance = limit;
            Limit = limit;
            Type = AccountTypes.Credit;
            Commission = commission;
        }

        public double Limit { get; }
        public double Commission { get; }
        public string Type { get; }
        public double Balance { get; private set; }
        public bool IsSuspicious { get; private set; }
        public Guid Id { get; }

        public bool IsTransferable()
        {
            return true;
        }

        public void UpdateBalance(double funds)
        {
            if (funds < 0)
                throw new BanksException("Funds can't be negative.");

            Balance += funds;
        }

        public void WithDrawFunds(double funds)
        {
            if (funds < 0)
                throw new BanksException("Funds can't be negative.");

            if (Balance - Limit >= 0)
                Balance -= funds;
            else
                Balance -= funds + Commission;
        }

        public void SetSuspiciousStatus(bool status)
        {
            IsSuspicious = status;
        }

        public void AddPercents()
        {
        }
    }
}