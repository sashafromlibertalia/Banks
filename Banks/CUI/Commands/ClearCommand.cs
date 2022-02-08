using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class ClearCommand : ICommand
    {
        public ClearCommand(CommandData commandData)
        {
            commandData.NotNull("Command data is null.");

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
            AnsiConsole.Console.Clear();
        }

        public void SetArgument(string argument)
        {
        }
    }
}