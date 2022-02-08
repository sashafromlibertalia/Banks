using System;
using Banks.Services;

namespace Banks.Entities
{
    public class Transaction : IEquatable<Transaction>
    {
        public Transaction(IAccount from, IAccount to, double funds)
        {
            Sender = from;
            Receiver = to;
            Funds = funds;
            Time = DateTime.Now;
            Id = Guid.NewGuid();
        }

        public Guid Id { get; }
        public IAccount Sender { get; }
        public IAccount Receiver { get; }
        public double Funds { get; }
        public DateTime Time { get; }

        public void Revert()
        {
            Sender?.UpdateBalance(Funds);
            Receiver?.WithDrawFunds(Funds);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Time);
        }

        public override bool Equals(object obj) => Equals(obj as Transaction);

        public bool Equals(Transaction other)
        {
            if (other == null)
                return false;

            return Id == other.Id &&
                   Time == other.Time;
        }
    }
}