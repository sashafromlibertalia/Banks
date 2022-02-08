using System.Collections.Generic;
using System.Linq;
using Banks.CUI.Commands;
using Banks.Entities;
using Banks.Services;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.States
{
    public class ClientsListState : State
    {
        private List<ICommand> _commands;
        public ClientsListState()
        {
        }

        public override void ChangeCommands(ICentralBank centralBank, ICommandHandler commandHandler)
        {
            if (centralBank.Clients.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{StylesInfo.DangerColor}]Clients list is empty.[/]");
                return;
            }

            Table clientsData = new Table().Expand().Centered().Title(new TableTitle($"[reverse {StylesInfo.MainColor}]  Clients  [/]"))
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]ID[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Bank[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Name[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Suspicious[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Registered accounts[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Passport[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Address[/]").Centered());

            foreach ((Client client, int index) pair in centralBank.Clients.Select((client, index) => (client, index)))
            {
                clientsData.AddRow(
                    $"{pair.index + 1}",
                    centralBank.Banks.Single(item => item.Clients.Any(currentClient => currentClient.Equals(pair.client))).Name,
                    pair.client.FullName,
                    $"{pair.client.Accounts[0].IsSuspicious}",
                    $"{pair.client.Accounts.Count}",
                    pair.client.Passport.Length == 0 ? "-" : pair.client.Passport,
                    pair.client.Address.Length == 0 ? "-" : pair.client.Address);
            }

            AnsiConsole.Console.Clear();
            _commands = new List<ICommand>
            {
                new ClientPreviewCommand(new CommandData("/show", "Shows data for the specified client. Provide number from a table."), centralBank),
                new ExitPreviewCommand(new CommandData("/exit", "Exits from client preview to a main screen."), Context),
            };
            commandHandler.UpdateCommands(_commands);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(clientsData);
        }
    }
}