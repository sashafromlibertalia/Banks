using Banks.Entities;
using Banks.Services;

namespace Banks.CUI
{
    public abstract class State
    {
        protected Context Context { get; private set; }
        public void SetContext(Context context)
        {
            Context = context;
        }

        public abstract void ChangeCommands(ICentralBank centralBank, ICommandHandler commandHandler);
    }
}