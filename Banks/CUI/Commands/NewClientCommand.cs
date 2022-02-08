using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Abstract;
using Banks.Accounts;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class NewClientCommand : ICommand
    {
        private readonly ICentralBank _centralBank;
        private readonly ClientBuilder _clientBuilder;

        public NewClientCommand(ICentralBank centralBank, CommandData commandData)
        {
            centralBank.NotNull("Central bank is null.");
            commandData.NotNull("Command data is null.");

            _clientBuilder = new ClientBuilder();
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
                if (_centralBank.Banks.Count == 0)
                {
                    AnsiConsole.Write(
                        new Markup($"[{StylesInfo.DangerColor}]Can't create client without registered banks.[/]"));
                    AnsiConsole.WriteLine();
                    return;
                }

                AnsiConsole.Write(new Rule($"[reverse {StylesInfo.ItemCreationColor}]  New client creation  [/]")
                    .LeftAligned().RoundedBorder().RuleStyle("grey"));
                string name = AnsiConsole.Ask<string>($"What's your [{StylesInfo.AccentColor}]name[/]?");
                string surname = AnsiConsole.Ask<string>($"What's your [{StylesInfo.AccentColor}]surname[/]?");

                string address = AnsiConsole
                    .Prompt(new TextPrompt<string>(
                            $"[{StylesInfo.NotificationWrapperColor}][[Optional]][/] What's your [{StylesInfo.AccentColor}]address[/]?")
                        .AllowEmpty());
                string passport = AnsiConsole
                    .Prompt(new TextPrompt<string>(
                            $"[{StylesInfo.NotificationWrapperColor}][[Optional]][/] What's your [{StylesInfo.AccentColor}]passport[/]?")
                        .AllowEmpty());

                SelectionPrompt<string> bankPrompt = new SelectionPrompt<string>()
                    .Title($"Please, select [{StylesInfo.AccentColor}]bank[/] you want to join.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor);
                foreach (Bank item in _centralBank.Banks)
                {
                    bankPrompt.AddChoice(item.Name);
                }

                string selectedBank = AnsiConsole.Prompt(bankPrompt);
                Bank foundBank = _centralBank.Banks.Single(item => item.Name == selectedBank);

                MultiSelectionPrompt<string> accountPrompt = new MultiSelectionPrompt<string>()
                    .Title($"Please, select [{StylesInfo.AccentColor}]account types[/] you want to add.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor)
                    .AddChoices(AccountTypes.Credit, AccountTypes.Debit, AccountTypes.Deposit);

                var addedAccounts = new List<IAccount>();

                foreach (string selectedAccount in AnsiConsole.Prompt(accountPrompt))
                {
                    AnsiConsole.WriteLine($"{selectedAccount} account was added.");
                    switch (selectedAccount)
                    {
                        case AccountTypes.Credit:
                            IAccount credit = foundBank.CreateCreditAccount();
                            addedAccounts.Add(credit);
                            break;
                        case AccountTypes.Debit:
                            IAccount debit = foundBank.CreateDebitAccount();
                            addedAccounts.Add(debit);
                            break;
                        case AccountTypes.Deposit:
                            double moneyForDeposit = AnsiConsole.Ask<double>(
                                $"How much [{StylesInfo.AccentColor}]money[/] you want to put on the deposit?");
                            int months = AnsiConsole.Ask<int>(
                                $"Provide [{StylesInfo.AccentColor}]months[/] you want to open your deposit?");
                            int years = AnsiConsole.Ask<int>(
                                $"Provide [{StylesInfo.AccentColor}]years[/] you want to open your deposit?");
                            IAccount deposit = foundBank.CreateDepositAccount(new HumanDate(months, years), moneyForDeposit);
                            addedAccounts.Add(deposit);
                            break;
                    }
                }

                Client client = _centralBank.RegisterClient(_clientBuilder
                    .AddName(name)
                    .AddSurname(surname)
                    .AddAddress(address)
                    .AddPassport(passport)
                    .AddAccounts(addedAccounts)
                    .Build());
                foundBank.AddClient(client);

                AnsiConsole.MarkupLine($"[reverse {StylesInfo.ItemCreationColor}] New client added successfully  [/]");
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