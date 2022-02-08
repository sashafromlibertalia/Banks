using System.Collections.Generic;
using Banks.CUI.Commands;
using Banks.Entities;
using Banks.Services;
using Banks.Types;

namespace Banks.CUI.States
{
    public class MainState : State
    {
        private List<ICommand> _commands;
        public MainState()
        {
        }

        public override void ChangeCommands(ICentralBank centralBank, ICommandHandler commandHandler)
        {
            _commands = new List<ICommand>
            {
                new NewBankCommand(centralBank, new CommandData("/new-bank", "Creates a new bank.")),
                new NewClientCommand(centralBank, new CommandData("/new-client", "Creates a new client.")),
                new GetBanksCommand(new CommandData("/banks", "Displays list of registered banks."), Context),
                new GetClientsCommand(Context, new CommandData("/clients", "Displays list of registered clients.")),
                new ClearCommand(new CommandData("/clear", "Clears program output.")),
                new QuitCommand(new CommandData("/quit", "Stops program.")),
            };
            commandHandler.UpdateCommands(_commands);
        }
    }
}