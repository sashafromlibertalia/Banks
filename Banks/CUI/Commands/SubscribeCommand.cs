using Banks.Entities;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class SubscribeCommand : ICommand
    {
        private readonly Bank _bank;
        public SubscribeCommand(CommandData commandData, Bank bank)
        {
            bank.NotNull("Bank is null.");
            commandData.NotNull("Command data is null.");

            Name = commandData.Name;
            Description = commandData.Description;
            IsParameterAvailable = true;
            _bank = bank;
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
                ISubscriber subscriber = _bank.Clients[int.Parse(ProvidedArgument) - 1];
                _bank.AddSubscriber(subscriber);
                AnsiConsole.MarkupLine($"[reverse {StylesInfo.ItemCreationColor}]  Client #{ProvidedArgument} was successfully subscribed to notifications.  [/]");
                AnsiConsole.WriteLine();
            }
            catch
            {
                AnsiConsole.Write(new Rule($"[{StylesInfo.DangerColor}]This client has notifications already.[/]")
                    .RuleStyle(StylesInfo.NotificationWrapperColor)
                    .LeftAligned());
                AnsiConsole.WriteLine();
            }
        }

        public void SetArgument(string argument)
        {
            ProvidedArgument = argument;
        }
    }
}