using System;
using System.Linq;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class BalanceInTimeCommand : ICommand
    {
        private readonly Bank _bank;

        public BalanceInTimeCommand(CommandData commandData, Bank bank)
        {
            commandData.NotNull("Command data is null.");
            bank.NotNull("Bank is null.");

            _bank = bank;
            Name = commandData.Name;
            Description = commandData.Description;
            IsParameterAvailable = true;
            ProvidedArgument = null;
        }

        public string Name { get; }
        public string Description { get; }
        public bool IsParameterAvailable { get; }
        public string ProvidedArgument { get; private set; }

        public void Execute()
        {
            try
            {
                var dateViewer = new DateViewer();
                Client client = _bank.Clients[int.Parse(ProvidedArgument) - 1];

                SelectionPrompt<string> accountPromptHandler = new SelectionPrompt<string>()
                    .Title($"Please, select [{StylesInfo.AccentColor}]account[/] you want to check.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor);

                foreach (IAccount account in client.Accounts)
                {
                    accountPromptHandler.AddChoice(account.Type);
                }

                string accountPrompt = AnsiConsole.Prompt(accountPromptHandler);
                IAccount selectedAccount = client.Accounts.Single(item => item.Type == accountPrompt);

                string datePrompt = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title($"Please, select [{StylesInfo.AccentColor}]date range[/] you want to execute.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor).AddChoices(ViewDateActions.Tomorrow, ViewDateActions.Month, ViewDateActions.Year, ViewDateActions.Custom));
                switch (datePrompt)
                {
                    case ViewDateActions.Tomorrow:
                        dateViewer.OneDay();
                        AnsiConsole.MarkupLine(
                            $"{selectedAccount.Id} will have {_bank.ViewBalanceInTime(dateViewer, selectedAccount)} {selectedAccount.Currency} on {dateViewer.Date}");
                        AnsiConsole.WriteLine();
                        break;
                    case ViewDateActions.Month:
                        dateViewer.OneMonth();
                        AnsiConsole.MarkupLine(
                            $"{selectedAccount.Id} will have {_bank.ViewBalanceInTime(dateViewer, selectedAccount)} {selectedAccount.Currency} on {dateViewer.Date}");
                        AnsiConsole.WriteLine();
                        break;
                    case ViewDateActions.Year:
                        dateViewer.OneYear();
                        AnsiConsole.MarkupLine(
                            $"{selectedAccount.Id} will have {_bank.ViewBalanceInTime(dateViewer, selectedAccount)} {selectedAccount.Currency} on {dateViewer.Date}");
                        AnsiConsole.WriteLine();
                        break;
                    case ViewDateActions.Custom:
                        int day = AnsiConsole.Ask<int>($"Provide a [{StylesInfo.AccentColor}]day[/]");
                        int month = AnsiConsole.Ask<int>($"Provide a [{StylesInfo.AccentColor}]month[/]");
                        int year = AnsiConsole.Ask<int>($"Provide a [{StylesInfo.AccentColor}]year[/]");
                        dateViewer.OnDate(day, month, year);
                        AnsiConsole.MarkupLine(
                            $"{selectedAccount.Id} will have {_bank.ViewBalanceInTime(dateViewer, selectedAccount)} {selectedAccount.Currency} on {dateViewer.Date}");
                        AnsiConsole.WriteLine();
                        break;
                }
            }
            catch (Exception e)
            {
                AnsiConsole.WriteException(e);
            }
        }

        public void SetArgument(string argument)
        {
            ProvidedArgument = argument;
        }
    }
}