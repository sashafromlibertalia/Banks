using System;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class NewBankCommand : ICommand
    {
        private readonly ICentralBank _centralBank;
        public NewBankCommand(ICentralBank centralBank, CommandData commandData)
        {
            centralBank.NotNull("Central bank is null.");
            commandData.NotNull("Command data is null.");

            _centralBank = centralBank;
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
                AnsiConsole.Write(new Rule($"[reverse {StylesInfo.ItemCreationColor}]  New bank creation  [/]").LeftAligned().RoundedBorder().RuleStyle(StylesInfo.NotificationWrapperColor));
                string name = AnsiConsole.Ask<string>($"What's bank [{StylesInfo.AccentColor}]name[/]?");
                AnsiConsole.WriteLine();
                double creditLimit = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]credit limit[/]?");
                double creditCommission = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]credit commission[/]?");
                AnsiConsole.WriteLine();
                double debitPercentage = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]debit percentage[/]?");
                AnsiConsole.WriteLine();
                double depositPercentage = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]deposit percentage[/]?");
                AnsiConsole.WriteLine();
                double transferLimit = AnsiConsole.Ask<double>($"What's bank's [{StylesInfo.AccentColor}]transfer limit[/]?");
                _centralBank.RegisterBank(new Bank(new BankData(name, creditLimit, creditCommission, debitPercentage, depositPercentage, transferLimit)));
                AnsiConsole.MarkupLine($"[reverse {StylesInfo.ItemCreationColor}]  New bank added successfully  [/]");
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