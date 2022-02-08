namespace Banks.Services
{
    public interface ICommand
    {
        string Name { get; }
        string Description { get; }
        bool IsParameterAvailable { get; }

        void Execute();
        void SetArgument(string argument);
    }
}