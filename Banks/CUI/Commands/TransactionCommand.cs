using System;
using System.Linq;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class TransactionCommand : ICommand
    {
        private readonly Bank _bank;
        private readonly ICentralBank _centralBank;

        public TransactionCommand(CommandData commandData, Bank bank, ICentralBank centralBank)
        {
            commandData.NotNull("Command data is null.");
            bank.NotNull("Bank is null.");
            centralBank.NotNull("Central bank is null.");

            _centralBank = centralBank;
            _bank = bank;
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
            if (_bank.Clients.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{StylesInfo.DangerColor}]Clients list is empty.[/]");
                return;
            }

            try
            {
                Client sender = _bank.Clients[int.Parse(ProvidedArgument) - 1];
                SelectionPrompt<string> accountPromptHandler = new SelectionPrompt<string>()
                    .Title($"Please, select [{StylesInfo.AccentColor}]account[/] you want to check.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor);

                foreach (IAccount account in sender.Accounts)
                {
                    accountPromptHandler.AddChoice(account.Type);
                }

                string accountPrompt = AnsiConsole.Prompt(accountPromptHandler);
                AnsiConsole.MarkupLine($"[{StylesInfo.AccentColor}]{accountPrompt}[/] account was selected.");
                AnsiConsole.WriteLine();
                IAccount senderAccount = sender.Accounts.Single(item => item.Type == accountPrompt);

                SelectionPrompt<string> transactionPrompt = new SelectionPrompt<string>()
                    .Title($"Please, select [{StylesInfo.AccentColor}]transaction action[/] you want to do.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor)
                    .AddChoices(TransactionActions.Transfer, TransactionActions.Update, TransactionActions.Withdraw);

                string transactionType = AnsiConsole.Prompt(transactionPrompt);
                switch (transactionType)
                {
                    case TransactionActions.Transfer:

                        Table clientsData = new Table().Expand().Centered()
                            .Title(new TableTitle($"[reverse {StylesInfo.MainColor}]  Clients  [/]"))
                            .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]ID[/]").Centered())
                            .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Bank[/]").Centered())
                            .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Name[/]").Centered())
                            .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Suspicious[/]").Centered())
                            .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Registered accounts[/]").Centered())
                            .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Passport[/]").Centered())
                            .AddColumn(new TableColumn($"[b {StylesInfo.DataColor}]Address[/]").Centered());

                        var uniqueClients = _centralBank.Clients.Where(items => !items.Equals(sender)).ToList();
                        foreach ((Client client, int index) pair in uniqueClients.Select((client, index) =>
                                     (client, index)))
                        {
                            clientsData.AddRow(
                                $"{pair.index + 1}",
                                _centralBank.Banks.Select(item => item.Clients.Single(currentClient => currentClient.Equals(pair.client))).ToList()[0].Name,
                                pair.client.FullName,
                                $"{pair.client.Accounts[0].IsSuspicious}",
                                $"{pair.client.Accounts.Count}",
                                pair.client.Passport.Length == 0 ? "-" : pair.client.Passport,
                                pair.client.Address.Length == 0 ? "-" : pair.client.Address);
                        }

                        AnsiConsole.Write(clientsData);
                        AnsiConsole.WriteLine();
                        int receiverNumber =
                            AnsiConsole.Ask<int>($"[{StylesInfo.AccentColor}]Whom[/] you want to transfer funds?");
                        Client receiver = uniqueClients[receiverNumber - 1];
                        SelectionPrompt<string> receiverAccountPrompt = new SelectionPrompt<string>()
                            .Title($"Please, select [{StylesInfo.AccentColor}]account[/] of receiver.")
                            .PageSize(10)
                            .HighlightStyle(StylesInfo.AccentColor);

                        foreach (IAccount account in receiver.Accounts)
                        {
                            receiverAccountPrompt.AddChoice(account.Type);
                        }

                        string receiverAccountType = AnsiConsole.Prompt(receiverAccountPrompt);
                        IAccount receiverAccount = receiver.Accounts.Single(item => item.Type == receiverAccountType);
                        AnsiConsole.MarkupLine(
                            $"[{StylesInfo.AccentColor}]{receiverAccountType}[/] account was selected for receiver.");
                        AnsiConsole.WriteLine();

                        double transferFunds = AnsiConsole.Ask<double>(
                            $"How much [{StylesInfo.AccentColor}]money[/] do you want to send funds?");
                        _bank.TransferFunds(senderAccount, receiverAccount, transferFunds);
                        AnsiConsole.MarkupLine(
                            $"[reverse {StylesInfo.ItemCreationColor}]  Account {sender.Id} sent to {receiver.Name} {transferFunds} {senderAccount.Currency}  [/]");
                        AnsiConsole.WriteLine();
                        break;
                    case TransactionActions.Update:
                        double updatingFunds =
                            AnsiConsole.Ask<double>($"How much [{StylesInfo.AccentColor}]money[/] do you want to put?");

                        _bank.UpdateBalance(senderAccount, updatingFunds);
                        AnsiConsole.MarkupLine(
                            $"[reverse {StylesInfo.ItemCreationColor}]  Account {sender.Id} now has {senderAccount.Balance} {senderAccount.Currency}  [/]");
                        AnsiConsole.WriteLine();
                        break;
                    case TransactionActions.Withdraw:
                        double withdrawingFunds =
                            AnsiConsole.Ask<double>(
                                $"How much [{StylesInfo.AccentColor}]money[/] do you want to withdraw?");

                        _bank.WithDrawFunds(senderAccount, withdrawingFunds);
                        AnsiConsole.MarkupLine(
                            $"[reverse {StylesInfo.ItemCreationColor}]  Account {sender.Id} now has {senderAccount.Balance} {senderAccount.Currency}  [/]");
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