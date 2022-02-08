using System.Collections.Generic;
using Banks.Entities;

namespace Banks.Services
{
    public interface IClientBuilder
    {
        Client Build();
        IClientBuilder AddName(string name);
        IClientBuilder AddSurname(string surname);
        IClientBuilder AddAddress(string address);
        IClientBuilder AddPassport(string passport);
        IClientBuilder AddAccounts(List<IAccount> accounts);
    }
}