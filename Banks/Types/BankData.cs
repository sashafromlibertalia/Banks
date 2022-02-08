using System;
using Banks.Tools;

namespace Banks.Types
{
    public class BankData
    {
        private const double MaximalCreditLimit = 120000;
        private const double MaximumCreditCommission = 10000;
        private const double MaximumDebitPercentage = 5;
        private const double MaximumDepositPercentage = 5;
        private const double MinimalTransferLimit = 100000;

        public BankData(string name, double creditLimit, double creditCommission, double debitPercentage, double depositPercentage, double transferLimit)
        {
            if (creditCommission < 0 || debitPercentage < 0 || depositPercentage < 0 || creditLimit <= 0 || transferLimit < 0)
                throw new BanksException("Can't create bank data with specified arguments.");

            if (creditLimit > MaximalCreditLimit)
                throw new BanksException($"Can't create bank with specified credit limit. Maximum value is {MaximalCreditLimit}");

            if (creditCommission > MaximumCreditCommission)
                throw new BanksException($"Can't create bank with specified credit commission. Maximum value is {MaximumCreditCommission}");

            if (debitPercentage > MaximumDebitPercentage)
                throw new BanksException($"Can't create bank with specified debit percentage. Maximum value is {MaximumDebitPercentage}");

            if (depositPercentage > MaximumDepositPercentage)
                throw new BanksException($"Can't create bank with specified deposit percentage. Maximum value is {MaximumDepositPercentage}");

            if (transferLimit < MinimalTransferLimit)
                throw new BanksException($"Can't create bank with specified transfer limit. Minimal value is {MinimalTransferLimit}");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(string.Empty, "Invalid bank name.");

            CreditLimit = creditLimit;
            CreditCommission = creditCommission;
            DebitPercentage = debitPercentage;
            DepositPercentage = depositPercentage;
            TransferLimit = transferLimit;
            Name = name;
        }

        public string Name { get; }
        public double CreditLimit { get; }
        public double CreditCommission { get; }
        public double DebitPercentage { get; }
        public double DepositPercentage { get; }
        public double TransferLimit { get; }
    }
}