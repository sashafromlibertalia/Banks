using System.Collections.Generic;
using System.Linq;
using Banks.CUI.Commands;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.States
{
    public class ShowBankState : State
    {
        private readonly Bank _bank;

        private List<ICommand> _commands;
        public ShowBankState(Bank bank)
        {
            bank.NotNull("Provided bank is null.");
            _bank = bank;
        }

        public override void ChangeCommands(ICentralBank centralBank, ICommandHandler commandHandler)
        {
            AnsiConsole.Console.Clear();
            Table table = new Table()
                .Centered()
                .Expand()
                .Title($"[reverse {StylesInfo.MainColor}]  {_bank.Name} data  [/]")
                .Border(TableBorder.MinimalHeavyHead)
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Clients[/]").Centered());

            Table clientsData = new Table().Expand().Centered()
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]ID[/]").Centered().NoWrap())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Suspicious[/]").Centered().NoWrap())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Name[/]").Centered().NoWrap())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Registered accounts[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Passport[/]").Centered().NoWrap())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Address[/]").Centered().NoWrap());

            foreach (var item in _bank.Clients.Select((data, index) => new { data, index }))
            {
                Client client = item.data;
                IReadOnlyList<IAccount> accounts = item.data.Accounts;
                int index = item.index;
                bool status = string.IsNullOrEmpty(client.Address) && string.IsNullOrEmpty(client.Passport);

                clientsData.AddRow(
                    $"{index + 1}",
                    $"{status}",
                    client.FullName,
                    $"{accounts.Count}",
                    string.IsNullOrEmpty(client.Passport) ? "-" : client.Passport,
                    string.IsNullOrEmpty(client.Address) ? "-" : client.Address).Centered();
            }

            table.AddRow(clientsData);
            _commands = new List<ICommand>
            {
                new RefreshBankDataCommand(new CommandData("/refresh", "Refreshes bank data"), _bank),
                new SubscribeCommand(new CommandData("/subscribe", "Subscribes user to notifications about bank data changes. Provide number from a table."), _bank),
                new UpdateClientDataCommand(new CommandData("/update", "Updates data for the selected client. Provide number of client from table."), _bank),
                new ClientPreviewCommand(new CommandData("/show", "Shows data for the selected client. Provide number of client from table."), centralBank),
                new TransactionCommand(new CommandData("/transaction", "Creates a transaction action for specified client. Provide number of client from table."), _bank, centralBank),
                new BalanceInTimeCommand(new CommandData("/time", "Shows balance for the selected client in time. Provide number of client from table."), _bank),
                new RevertTransaction(new CommandData("/revert", "Reverts a transaction specified by a client."), _bank),
                new ExitPreviewCommand(new CommandData("/exit", "Exits from bank preview to a main screen."), Context),
                new ClearCommand(new CommandData("/clear", "Clears program output.")),
            };
            commandHandler.UpdateCommands(_commands);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
        }
    }
}