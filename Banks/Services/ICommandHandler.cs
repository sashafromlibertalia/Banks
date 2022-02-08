using System.Collections.Generic;

namespace Banks.Services
{
    public interface ICommandHandler
    {
        void UpdateCommands(List<ICommand> commands);
        void Run();
    }
}