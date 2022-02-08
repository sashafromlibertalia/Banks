using System;
using Banks.Tools;

namespace Banks.Types
{
    public class CommandData
    {
        private const int MinimalCommandNameLength = 4;
        public CommandData(string name, string description)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description))
                throw new ArgumentNullException(string.Empty, "Invalid command data.");

            if (name.Substring(0, 1) != "/" || name.Length + 1 < MinimalCommandNameLength)
                throw new BanksException("Invalid command name.");

            Name = name;
            Description = description;
            PassedArgument = null;
        }

        public string Name { get; }
        public string Description { get; }
        public string PassedArgument { get; }
    }
}