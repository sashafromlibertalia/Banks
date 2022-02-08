using Banks.CUI.States;
using Banks.Services;
using Banks.Tools;
using Banks.Types;

namespace Banks.CUI.Commands
{
    public class GetClientsCommand : ICommand
    {
        private readonly Context _context;
        public GetClientsCommand(Context context, CommandData commandData)
        {
            context.NotNull("Context is null.");
            commandData.NotNull("Command data is null.");

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
            _context.TransitionTo(new ClientsListState());
            _context.ChangeCommands();
        }

        public void SetArgument(string argument)
        {
        }
    }
}