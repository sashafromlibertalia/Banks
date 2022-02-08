using System.Collections.Generic;
using Banks.Entities;

namespace Banks.Services
{
    public interface ICentralBank
    {
        IReadOnlyList<Bank> Banks { get; }
        IReadOnlyList<Client> Clients { get; }
        Bank RegisterBank(Bank bank);
        Client RegisterClient(Client client);
        void NotifyBanks();
        Bank GetBankByClient(Client client);
    }
}