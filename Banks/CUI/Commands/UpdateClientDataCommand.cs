using System;
using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class UpdateClientDataCommand : ICommand
    {
        private readonly Bank _bank;
        public UpdateClientDataCommand(CommandData commandData, Bank bank)
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
            if (_bank.Clients.Count == 0)
            {
                AnsiConsole.MarkupLine($"[{StylesInfo.DangerColor}]Clients list is empty.[/]");
                return;
            }

            try
            {
                Client client = _bank.Clients[int.Parse(ProvidedArgument) - 1];
                if (!string.IsNullOrEmpty(client.Address) && !string.IsNullOrEmpty(client.Passport))
                {
                    AnsiConsole.MarkupLine($"[reverse ${StylesInfo.MainColor}]  {client.FullName} already has all necessary data.  [/]");
                    return;
                }

                SelectionPrompt<string> dataToChangePrompt = new SelectionPrompt<string>()
                    .Title($"Please, select which [{StylesInfo.AccentColor}]data[/] you want to change.")
                    .PageSize(10)
                    .HighlightStyle(StylesInfo.AccentColor);
                dataToChangePrompt.AddChoices("Address", "Passport");
                switch (AnsiConsole.Prompt(dataToChangePrompt))
                {
                    case "Address":
                        string address = AnsiConsole.Ask<string>($"What's your [{StylesInfo.AccentColor}]address[/]?");
                        client.UpdateAddress(address);
                        break;
                    case "Passport":
                        string passport = AnsiConsole.Ask<string>($"What's your [{StylesInfo.AccentColor}]passport[/]?");
                        client.UpdatePassport(passport);
                        break;
                }

                AnsiConsole.WriteLine("Now client's account isn't suspicious!");
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