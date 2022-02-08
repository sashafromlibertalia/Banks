using System.Collections.Generic;
using System.Linq;
using Banks.Services;
using Banks.Tools;

namespace Banks.Entities
{
    public class CentralBank : ICentralBank
    {
        private readonly List<Bank> _banks;
        private readonly List<Client> _clients;
        public CentralBank()
        {
            _banks = new List<Bank>();
            _clients = new List<Client>();
        }

        public IReadOnlyList<Bank> Banks => _banks;
        public IReadOnlyList<Client> Clients => _clients;

        public Client RegisterClient(Client client)
        {
            client.NotNull("Added client is null.");
            _clients.WithoutDuplicates(client, "This client already added.");

            _clients.Add(client);
            return client;
        }

        public void NotifyBanks()
        {
            foreach (Bank bank in _banks)
            {
                bank.CreatePayments();
            }
        }

        public Bank GetBankByClient(Client client)
        {
            return _banks.FirstOrDefault(bank => bank.Clients.Any(registered => registered.Equals(client)));
        }

        public Bank RegisterBank(Bank bank)
        {
            bank.NotNull("Added bank is null.");
            _banks.WithoutDuplicates(bank, "This bank is already added.");

            _banks.Add(bank);
            return bank;
        }
    }
}