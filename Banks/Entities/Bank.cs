using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Accounts;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.Entities
{
    public class Bank : INotifier, IEquatable<Bank>
    {
        private readonly List<IAccount> _accounts;
        private readonly List<Client> _clients;
        private readonly List<Transaction> _revertedTransactions;
        private readonly List<ISubscriber> _subscribers;
        private readonly List<Transaction> _transactions;
        private readonly CreditCreator _creditCreator;
        private readonly DebitCreator _debitCreator;
        private BankData _bankData;
        public Bank(BankData bankData)
        {
            bankData.NotNull("Bank data is null.");

            _accounts = new List<IAccount>();
            _clients = new List<Client>();
            _transactions = new List<Transaction>();
            _subscribers = new List<ISubscriber>();
            _revertedTransactions = new List<Transaction>();
            _creditCreator = new CreditCreator(bankData.CreditLimit, bankData.CreditCommission);
            _debitCreator = new DebitCreator(bankData.DebitPercentage);
            _bankData = bankData;
            State = bankData;
        }

        public string Name => _bankData.Name;
        public double CreditLimit => _bankData.CreditLimit;
        public double CreditCommission => _bankData.CreditCommission;
        public double DebitPercentage => _bankData.DebitPercentage;
        public double DepositPercentage => _bankData.DepositPercentage;
        public double TransferLimit => _bankData.TransferLimit;
        public BankData State { get; }
        public IReadOnlyList<Transaction> Transactions => _transactions;
        public IReadOnlyList<Transaction> RevertedTransaction => _revertedTransactions;
        public IReadOnlyList<Client> Clients => _clients;

        public void AddClient(Client client)
        {
            client.Accounts.NotEmpty("Account list is empty.");
            client.NotNull("Client is null.");
            _accounts.AddRange(client.Accounts);

            _clients.Add(client);
        }

        public IAccount CreateCreditAccount()
        {
            return _creditCreator.CreateAccount();
        }

        public IAccount CreateDebitAccount()
        {
            return _debitCreator.CreateAccount();
        }

        public IAccount CreateDepositAccount(HumanDate humanDate, double moneyForDeposit)
        {
            var depositCreator = new DepositCreator(humanDate, moneyForDeposit, DepositPercentage);
            return depositCreator.CreateAccount();
        }

        public Transaction TransferFunds(IAccount sender, IAccount receiver, double funds)
        {
            sender.NotNull("Sender account is null.");
            receiver.NotNull("Receiver account is null.");
            _accounts.AddedToCollection(sender, $"This account isn't a part of {Name} bank.");

            if (funds < 0)
                throw new BanksException("Can't transfer negative amount of funds.");

            if (sender.IsTransferable())
            {
                if (sender.IsSuspicious && funds > TransferLimit)
                    throw new BanksException($"This account is suspicious. You can't transfer more than {TransferLimit} {sender.Currency}");

                sender.WithDrawFunds(funds);
                receiver.UpdateBalance(funds);

                var transaction = new Transaction(sender, receiver, funds);
                _transactions.Add(transaction);
                return transaction;
            }

            throw new BanksException($"Can't create a transfer operation. {sender.Type} account doesn't support it yet.");
        }

        public Transaction UpdateBalance(IAccount sender, double funds)
        {
            sender.NotNull("Sender account is null.");
            _accounts.AddedToCollection(sender, $"This account isn't a part of {Name} bank.");

            if (funds < 0)
                throw new BanksException("Can't update negative amount of funds.");

            if (sender.IsSuspicious && funds > TransferLimit)
                throw new BanksException($"This account is suspicious. You can't transfer more than {TransferLimit} {sender.Currency}");

            sender.UpdateBalance(funds);
            var transaction = new Transaction(null, sender, funds);
            _transactions.Add(transaction);
            return transaction;
        }

        public Transaction WithDrawFunds(IAccount sender, double funds)
        {
            sender.NotNull("Sender account is null.");
            _accounts.AddedToCollection(sender, $"This account isn't a part of {Name} bank.");

            if (funds < 0)
                throw new BanksException("Can't withdraw negative amount of funds.");

            if (sender.IsSuspicious && funds > TransferLimit)
                throw new BanksException($"This account is suspicious. You can't transfer more than {TransferLimit} {sender.Currency}");

            sender.WithDrawFunds(funds);
            var transaction = new Transaction(sender, null, funds);
            _transactions.Add(transaction);
            return transaction;
        }

        public void RevertTransaction(Transaction transaction)
        {
            transaction.NotNull("Transaction is null.");
            _transactions.AddedToCollection(transaction, $"This transaction wasn't registered in {Name} bank.");
            _revertedTransactions.WithoutDuplicates(transaction, "This transaction was already reverted.");

            transaction.Revert();
            _revertedTransactions.Add(transaction);
        }

        public void AddSubscriber(ISubscriber subscriber)
        {
            _subscribers.WithoutDuplicates(subscriber, "This client was already added to the notification system.");
            _subscribers.Add(subscriber);
        }

        public void RemoveSubscriber(ISubscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public void UpdateData(BankData newBankData)
        {
            newBankData.NotNull("New bank data is null.");
            _bankData = newBankData;
        }

        public void Notify(string accountType)
        {
            var subscribers = _subscribers.SelectMany(item => item.Accounts.Where(account => account.Type == accountType), (item, type) => new { item, type });
            foreach (var subscriber in subscribers)
            {
                subscriber.item.FetchUpdates();
            }
        }

        public void CreatePayments()
        {
            foreach (IAccount account in _clients.SelectMany(client => client.Accounts))
            {
                switch (account)
                {
                    case Debit debit:
                        debit.AddPercents();
                        debit.UpdateBalance(debit.AddedPercents.Sum());
                        break;
                    case Deposit deposit:
                        deposit!.AddPercents();
                        deposit.UpdateBalance(deposit.AddedPercents.Sum());
                        break;
                }
            }
        }

        public double ViewBalanceInTime(DateViewer dateViewer, IAccount account)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = dateViewer.Date;
            switch (account)
            {
                case Debit debit:
                    while (startDate.AddDays(1) <= endDate)
                    {
                        startDate = startDate.AddDays(1);
                        debit!.AddPercents();
                    }

                    return debit!.Balance + debit.AddedPercents.Sum();
                case Deposit deposit:
                    while (startDate.AddDays(1) <= endDate)
                    {
                        startDate = startDate.AddDays(1);
                        deposit!.AddPercents();
                    }

                    return deposit!.Balance + deposit.AddedPercents.Sum();
            }

            throw new BanksException("Invalid account type provided.");
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, CreditCommission, DebitPercentage);
        }

        public override bool Equals(object obj) => Equals(obj as Bank);
        public bool Equals(Bank other)
        {
            if (other == null)
                return false;

            return Name == other.Name &&
                   Math.Abs(CreditCommission - other.CreditCommission) < 0.01 &&
                   Math.Abs(DebitPercentage - other.DebitPercentage) < 0.01 &&
                   Math.Abs(DepositPercentage - other.DepositPercentage) < 0.01;
        }
    }
}