using System;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class ClientPreviewCommand : ICommand
    {
        private readonly ICentralBank _centralBank;
        public ClientPreviewCommand(CommandData commandData, ICentralBank centralBank)
        {
            commandData.NotNull("Command data is null.");
            centralBank.NotNull("Central bank is null.");

            _centralBank = centralBank;
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
                Client client = _centralBank.Clients[int.Parse(ProvidedArgument) - 1];
                Tree root = new Tree($"[reverse {StylesInfo.MainColor}]  {client.FullName}  [/] data:")
                    .Guide(TreeGuide.Line);

                Bank bank = _centralBank.GetBankByClient(client);

                root.AddNode($"[b]Bank: [/]{bank.Name}");
                TreeNode accounts = root.AddNode($"[reverse {StylesInfo.DataColor}]  Opened accounts  [/]");
                foreach (IAccount account in client.Accounts)
                {
                    accounts.AddNode($"[reverse {StylesInfo.ItemCreationColor}]  {account.Type}  [/]")
                        .AddNodes($"[b]Balance: [/]{account.Balance} {account.Currency}", $"[b]ID: [/]{account.Id}");
                }

                AnsiConsole.Write(root);
                AnsiConsole.WriteLine();
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