using System;
using System.Collections.Generic;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;

namespace Banks.Abstract
{
    public class ClientBuilder : IClientBuilder
    {
        private string _name;
        private string _surname;
        private string _address;
        private string _passport;
        private List<IAccount> _accounts;

        public Client Build()
        {
            return new Client(_name, _surname, _address, _passport, _accounts);
        }

        public IClientBuilder AddName(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(string.Empty, "Argument of client can't be null.");

            _name = name;
            return this;
        }

        public IClientBuilder AddSurname(string surname)
        {
            if (string.IsNullOrEmpty(surname))
                throw new ArgumentNullException(string.Empty, "Argument of client can't be null.");

            _surname = surname;
            return this;
        }

        public IClientBuilder AddAddress(string address)
        {
            _address = address;
            return this;
        }

        public IClientBuilder AddPassport(string passport)
        {
            _passport = passport;
            return this;
        }

        public IClientBuilder AddAccounts(List<IAccount> accounts)
        {
            accounts.NotEmpty("Accounts of client can't be empty.");
            _accounts = accounts;
            return this;
        }
    }
}