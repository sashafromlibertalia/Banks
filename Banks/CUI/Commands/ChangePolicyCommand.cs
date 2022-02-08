using System;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class ChangePolicyCommand : ICommand
    {
        private readonly ICentralBank _centralBank;
        public ChangePolicyCommand(CommandData commandData, ICentralBank centralBank)
        {
            centralBank.NotNull("Central bank is null.");
            commandData.NotNull("Command data is null.");

            _centralBank = centralBank;
            Name = commandData.Name;
            Description = commandData.Description;
            IsParameterAvailable = true;
        }

        public string Name { get; }
        public string Description { get; }
        public bool IsParameterAvailable { get; }
        public string ProvidedArgument { get; private set; }
        public void Execute()
        {
            try
            {
                Bank bank = _centralBank.Banks[int.Parse(ProvidedArgument) - 1];
                SelectionPrompt<string> accountPrompt = new SelectionPrompt<string>()
                    .Title($"Please, select [{StylesInfo.AccentColor}]account type[/] you want to change.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor);
                accountPrompt.AddChoices(AccountTypes.Credit, AccountTypes.Debit, AccountTypes.Deposit);
                string selectedAccount = AnsiConsole.Prompt(accountPrompt);
                switch (selectedAccount)
                {
                    case AccountTypes.Credit:
                        double creditLimit = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]new credit limit[/]?");
                        double creditCommission = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]new credit commission[/]?");
                        bank.UpdateData(new BankData(bank.Name, creditLimit, creditCommission, bank.DebitPercentage, bank.DepositPercentage, bank.TransferLimit));
                        bank.Notify(AccountTypes.Credit);
                        break;
                    case AccountTypes.Debit:
                        double debitPercentage = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]new debit percentage[/]?");
                        bank.UpdateData(new BankData(bank.Name, bank.CreditLimit, bank.CreditCommission, debitPercentage, bank.DepositPercentage, bank.TransferLimit));
                        bank.Notify(AccountTypes.Debit);
                        break;
                    case AccountTypes.Deposit:
                        double depositPercentage = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]new deposit percentage[/]?");
                        bank.UpdateData(new BankData(bank.Name, bank.CreditLimit, bank.CreditCommission, bank.DebitPercentage, depositPercentage, bank.TransferLimit));
                        bank.Notify(AccountTypes.Deposit);
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