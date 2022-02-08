using System;
using System.Linq;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class RevertTransaction : ICommand
    {
        private readonly Bank _bank;
        public RevertTransaction(CommandData commandData, Bank bank)
        {
            commandData.NotNull("Command data is null.");
            bank.NotNull("Bank is null.");

            _bank = bank;
            Name = commandData.Name;
            Description = commandData.Description;
            IsParameterAvailable = false;
            ProvidedArgument = null;
        }

        public string Name { get; }
        public string Description { get; }
        public bool IsParameterAvailable { get; }
        public string ProvidedArgument { get; }
        public void Execute()
        {
            try
            {
                Table clientsData = new Table().Expand().Centered()
                    .Title(new TableTitle($"[reverse {StylesInfo.MainColor}]  Clients  [/]"))
                    .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]ID[/]").Centered())
                    .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Sender[/]").Centered())
                    .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Receiver[/]").Centered())
                    .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Funds[/]").Centered())
                    .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Date[/]").Centered())
                    .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]ID[/]").Centered());

                foreach ((Transaction transaction, int i) in _bank.Transactions.Select((trans, index) => (trans, index)))
                {
                    clientsData.AddRow(
                        $"{i + 1}",
                        $"{(transaction.Sender == null ? "-" : transaction.Sender.Id)}",
                        $"{(transaction.Receiver == null ? "-" : transaction.Receiver.Id)}",
                        $"{transaction.Funds}",
                        $"{transaction.Time}",
                        $"{transaction.Id}");
                }

                AnsiConsole.Write(clientsData);
                AnsiConsole.WriteLine();

                int transactionToRevert = AnsiConsole.Ask<int>($"Type [{StylesInfo.AccentColor}]transaction[/] you want to revert?");
                Transaction invoice = _bank.Transactions[transactionToRevert - 1];
                _bank.RevertTransaction(invoice);

                AnsiConsole.MarkupLine($"[reverse {StylesInfo.ItemCreationColor}]  Transaction #{invoice.Id} was successfully reverted.  [/]");
                AnsiConsole.WriteLine();
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }
        }

        public void SetArgument(string argument)
        {
        }
    }
}