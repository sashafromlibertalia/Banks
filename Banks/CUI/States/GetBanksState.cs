using System.Collections.Generic;
using System.Linq;
using Banks.CUI.Commands;
using Banks.Entities;
using Banks.Services;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.States
{
    public class GetBanksState : State
    {
        private List<ICommand> _commands;
        public GetBanksState()
        {
        }

        public override void ChangeCommands(ICentralBank centralBank, ICommandHandler commandHandler)
        {
            if (centralBank.Banks.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{StylesInfo.DangerColor}]Bank list is empty.[/]");
                return;
            }

            AnsiConsole.Console.Clear();
            Table table = new Table()
                .Centered()
                .Expand()
                .Title($"[reverse {StylesInfo.MainColor}]  Registered banks data  [/]")
                .Border(TableBorder.Square)
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]ID[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Name[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Transfer limit[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Credit commission[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Credit limit[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Debit percentage[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Deposit percentage[/]").Centered())
                .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Clients[/]").Centered());

            foreach ((Bank bank, int index) pair in centralBank.Banks.Select((bank, index) => (bank, index)))
            {
                table.AddRow(
                    $"{pair.index + 1}",
                    $"{pair.bank.Name}",
                    $"{pair.bank.TransferLimit}",
                    $"{pair.bank.CreditCommission}",
                    $"{pair.bank.CreditLimit}",
                    $"{pair.bank.DebitPercentage}",
                    $"{pair.bank.DepositPercentage}",
                    $"{pair.bank.Clients.Count}");
            }

            _commands = new List<ICommand>
            {
                new BankPreviewCommand(new CommandData("/show", "Shows data for the selected bank. Provide number of bank from table."), Context),
                new ChangePolicyCommand(new CommandData("/change", "Changes bank policy, e.g. percentages and limits. Provide number of bank from table."), centralBank),
                new ExitPreviewCommand(new CommandData("/exit", "Exits from bank preview to a main screen."), Context),
                new ClearCommand(new CommandData("/clear", "Clears program output.")),
            };
            commandHandler.UpdateCommands(_commands);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(table);
        }
    }
}