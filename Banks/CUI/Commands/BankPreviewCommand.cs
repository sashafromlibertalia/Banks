using Banks.CUI.States;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.CUI.Commands
{
    public class BankPreviewCommand : ICommand
    {
        private readonly Context _context;
        public BankPreviewCommand(CommandData commandData, Context context)
        {
            commandData.NotNull("Command data is null.");
            context.NotNull("Added context is null.");

            _context = context;
            Name = commandData.Name;
            Description = commandData.Description;
            IsParameterAvailable = true;
        }

        public string Name { get; }
        public string Description { get; }
        public string ProvidedArgument { get; private set; }
        public bool IsParameterAvailable { get; }

        public void Execute()
        {
            _context.TransitionTo(new ShowBankState(_context.CentralBank.Banks[int.Parse(ProvidedArgument) - 1]));
            _context.ChangeCommands();
        }

        public void SetArgument(string argument)
        {
            ProvidedArgument = argument;
        }
    }
}