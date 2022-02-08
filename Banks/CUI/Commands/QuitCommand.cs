using System;
using Banks.Services;
using Banks.Tools;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI.Commands
{
    public class QuitCommand : ICommand
    {
        public QuitCommand(CommandData commandData)
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
            AnsiConsole.Write(new Rule($"[reverse {StylesInfo.MainColor}]  Program will be stopped if you press CTRL + C!  [/]").RuleStyle(StylesInfo.NotificationWrapperColor).LeftAligned());
        }

        public void SetArgument(string argument)
        {
        }
    }
}