using System;
using System.Collections.Generic;
using Banks.Services;
using Banks.Types;
using Spectre.Console;

namespace Banks.Entities
{
    public class Client : ISubscriber, IEquatable<Client>
    {
        public Client(string name, string surname, string address, string passport, IReadOnlyList<IAccount> accounts)
        {
            Name = name;
            Surname = surname;
            Address = address;
            Passport = passport;
            Accounts = accounts;
            FullName = name + " " + surname;
            Id = Guid.NewGuid();

            foreach (IAccount account in accounts)
            {
                account.SetSuspiciousStatus(string.IsNullOrEmpty(passport) || string.IsNullOrEmpty(address));
            }
        }

        public string Name { get; }
        public string Surname { get; }
        public string FullName { get; }
        public Guid Id { get; }
        public string Passport { get; private set; }
        public IReadOnlyList<IAccount> Accounts { get; }
        public string Address { get; private set; }

        public void FetchUpdates()
        {
            AnsiConsole.MarkupLine($"[reverse {StylesInfo.ItemCreationColor}]  Client {FullName} got updated data.  [/]");
            AnsiConsole.WriteLine();
        }

        public void UpdatePassport(string passport)
        {
            if (string.IsNullOrEmpty(Passport))
                Passport = passport;

            foreach (IAccount account in Accounts)
            {
                account.SetSuspiciousStatus(true);
            }
        }

        public void UpdateAddress(string address)
        {
            if (string.IsNullOrEmpty(Address))
                Address = address;

            foreach (IAccount account in Accounts)
            {
                account.SetSuspiciousStatus(true);
            }
        }

        public bool Equals(Client other)
        {
            if (other == null)
                return false;

            return Name == other.Name &&
                   Surname == other.Surname &&
                   Id == other.Id &&
                   Passport == other.Passport &&
                   Address == other.Address;
        }

        public override bool Equals(object obj) => Equals(obj as Client);

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Surname, Passport, Address, Id, Accounts);
        }
    }
}