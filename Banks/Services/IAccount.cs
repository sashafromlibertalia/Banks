using System;
using System.Collections.Generic;

namespace Banks.Services
{
    public interface IAccount
    {
        string Type { get; }
        string Currency => "₽";
        double Balance { get; }
        Guid Id { get; }
        bool IsSuspicious { get; }
        bool IsTransferable();
        void UpdateBalance(double funds);
        void WithDrawFunds(double funds);
        void SetSuspiciousStatus(bool status);
        void AddPercents();
    }
}