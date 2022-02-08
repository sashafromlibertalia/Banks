using System;
using System.Collections.Generic;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Entities
{
    public class Deposit : IAccount
    {
        private const double InitialMoney = 50000;
        private const double SecondInitialMoney = 100000;
        private const double InitialPercents = 0.5;
        private const double FinalInitialPercents = 1.5;
        private readonly List<double> _addedPercents;
        public Deposit(HumanDate date, double money, double percentage)
        {
            if (percentage < 0 || money < 0)
                throw new BanksException("Can't create account with specified arguments.");

            if (money < InitialMoney)
                Percentage = percentage;
            else if (money >= InitialMoney && money < SecondInitialMoney)
                Percentage = percentage + InitialPercents;
            else
                Percentage = percentage + FinalInitialPercents;

            Id = Guid.NewGuid();
            Balance = money;
            Type = AccountTypes.Deposit;
            ExpirationDate = date.Date;
            _addedPercents = new List<double>();
        }

        public DateTime ExpirationDate { get; }
        public bool IsSuspicious { get; private set; }
        public double Percentage { get; }
        public IReadOnlyList<double> AddedPercents => _addedPercents;
        public string Type { get; }
        public double Balance { get; private set; }
        public Guid Id { get; }

        public bool IsTransferable()
        {
            if (DateTime.Now < ExpirationDate)
                return false;

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
            if (DateTime.Now < ExpirationDate)
                throw new BanksException($"You can't withdraw funds from {Type} account until {ExpirationDate.Date}");

            if (funds < 0)
                throw new BanksException("Can't transfer negative amount of funds.");

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