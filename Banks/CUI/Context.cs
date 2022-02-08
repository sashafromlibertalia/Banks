using Banks.Entities;
using Banks.Services;

namespace Banks.CUI
{
    public class Context
    {
        private State _state;
        public Context(State state, ICentralBank centralBank)
        {
            CentralBank = centralBank;
            CommandHandler = new CommandHandler();
            TransitionTo(state);
        }

        public ICentralBank CentralBank { get; }
        public ICommandHandler CommandHandler { get; }

        public void TransitionTo(State state)
        {
            _state = state;
            _state.SetContext(this);
        }

        public void ChangeCommands()
        {
            _state.ChangeCommands(CentralBank, CommandHandler);
            CommandHandler.Run();
        }
    }
}