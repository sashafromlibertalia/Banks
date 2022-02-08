using Banks.Entities;
using Banks.Tools;
using Banks.Types;
using NUnit.Framework;

namespace Banks.Test
{
    [TestFixture]
    public class BanksTest
    {
        private CentralBank _centralBank;

        [SetUp]
        public void Setup()
        {
            _centralBank = new CentralBank();
        }

        [Test]
        public void CreateBankAndClient_ClientAddedToBank()
        {
            var bank = _centralBank.RegisterBank(new Bank(new BankData("Тинькофф", 50000, 1000, 2.5, 2.5, 100000)));
            var client = _centralBank.RegisterClient(new Client("Nana", "Mouskouri", null, null,
                new[] { new Debit(bank.DebitPercentage) }));
            bank.AddClient(client);
            Assert.AreEqual(1, bank.Clients.Count);
            Assert.AreEqual(1, _centralBank.Clients.Count);
            Assert.AreEqual(1, _centralBank.Banks.Count);
        }

        [Test]
        public void WithDrawMoneyFromDeposit_ThrowException()
        {
            var bank = _centralBank.RegisterBank(new Bank(new BankData("Тинькофф", 50000,  1000, 2.5, 2.5,
                100000)));
            var client = _centralBank.RegisterClient(new Client("Nana", "Mouskouri", null, null,
                new[] { new Deposit(new HumanDate(2, 0),10000, bank.DepositPercentage) }));
            bank.AddClient(client);
            Assert.Catch<BanksException>(() =>
            {
                client.Accounts[0].WithDrawFunds(100);
            });
        }

        [Test]
        public void TransferFromDebitToDebit_BothBalancesChanged()
        {
            var bank1 = _centralBank.RegisterBank(new Bank(new BankData("Тинькофф", 50000, 1000, 2.5, 2.5,
                100000)));
            var bank2 = _centralBank.RegisterBank(new Bank(new BankData("Сбербанк", 50000, 1000, 2.5, 2.5,
                100000)));
            var client1 = _centralBank.RegisterClient(new Client("Nana", "Mouskouri", null, null,
                new[] { new Debit(bank1.DebitPercentage) }));
            var client2 = _centralBank.RegisterClient(new Client("Demis", "Roussos", null, null,
                new[] { new Debit(bank1.DebitPercentage) }));
            bank1.AddClient(client1);
            bank2.AddClient(client2);

            client1.Accounts[0].UpdateBalance(100000);
            Assert.AreEqual(100000, client1.Accounts[0].Balance);
            bank1.TransferFunds(client1.Accounts[0], client2.Accounts[0], 5000);
            Assert.AreEqual(95000, client1.Accounts[0].Balance);
            Assert.AreEqual(5000, client2.Accounts[0].Balance);
        }

        [Test]
        public void TransferMoneyAndRevertTransaction_BalanceIsSame()
        {
            var bank1 = _centralBank.RegisterBank(new Bank(new BankData("Тинькофф", 50000, 1000, 2.5, 2.5,
                100000)));
            var bank2 = _centralBank.RegisterBank(new Bank(new BankData("Сбербанк", 50000, 1000, 2.5, 2.5,
                100000)));
            var client1 = _centralBank.RegisterClient(new Client("Nana", "Mouskouri", null, null,
                new[] { new Debit(bank1.DebitPercentage) }));
            var client2 = _centralBank.RegisterClient(new Client("Demis", "Roussos", null, null,
                new[] { new Debit(bank1.DebitPercentage) }));
            bank1.AddClient(client1);
            bank2.AddClient(client2);

            client1.Accounts[0].UpdateBalance(100000);
            var transaction = bank1.TransferFunds(client1.Accounts[0], client2.Accounts[0], 5000);
            bank1.RevertTransaction(transaction);

            Assert.AreEqual(1, bank1.RevertedTransaction.Count);
            Assert.AreEqual(100000, client1.Accounts[0].Balance);
            Assert.AreNotEqual(5000, client2.Accounts[0].Balance);
            Assert.AreEqual(0, client2.Accounts[0].Balance);
        }

        [Test]
        public void AddMoneyToDebitAccountAndCreatePayments_PercentsAreAdded()
        {
            var bank = _centralBank.RegisterBank(new Bank(new BankData("Тинькофф", 50000, 1000, 3.65, 2.5,
                100000)));
            var client = _centralBank.RegisterClient(new Client("Nana", "Mouskouri", null, null,
                new[] { new Debit(bank.DebitPercentage) }));

            bank.AddClient(client);
            client.Accounts[0].UpdateBalance(100000);
            Assert.AreEqual(100000, client.Accounts[0].Balance);

            bank.CreatePayments();
            Assert.AreEqual(100010, client.Accounts[0].Balance);
        }

        [Test]
        public void AddMoneyToDebitAndViewInTimeBalance_PercentsAreAdded()
        {
            var bank = _centralBank.RegisterBank(new Bank(new BankData("Тинькофф", 50000, 1000, 3.65, 2.5,
                100000)));
            var client = _centralBank.RegisterClient(new Client("Nana", "Mouskouri", null, null,
                new[] { new Debit(bank.DebitPercentage) }));

            var dateViewer = new DateViewer();
            dateViewer.OneMonth();
            bank.AddClient(client);
            client.Accounts[0].UpdateBalance(100000);
            Assert.AreEqual(100000, client.Accounts[0].Balance);
            Assert.AreEqual(100300, bank.ViewBalanceInTime(dateViewer, client.Accounts[0]));
        }
    }
}