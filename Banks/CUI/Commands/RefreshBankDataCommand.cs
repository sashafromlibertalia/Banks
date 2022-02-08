using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class RefreshBankDataCommand : ICommand
    {
        private readonly Bank _bank;
        public RefreshBankDataCommand(CommandData commandData, Bank bank)
        {
            commandData.NotNull("Command data is null.");
            bank.NotNull("Bank is null.");

            Name = commandData.Name;
            Description = commandData.Description;
            _bank = bank;
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
                AnsiConsole.WriteLine();
                AnsiConsole.Write(table);
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