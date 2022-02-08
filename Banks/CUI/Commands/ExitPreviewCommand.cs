using Banks.CUI.States;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class ExitPreviewCommand : ICommand
    {
        private readonly Context _context;
        public ExitPreviewCommand(CommandData commandData, Context context)
        {
            commandData.NotNull("Command data is null.");
            context.NotNull("Context is null.");

            _context = context;
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
            _context.TransitionTo(new MainState());
            _context.ChangeCommands();
        }

        public void SetArgument(string argument)
        {
        }
    }
}