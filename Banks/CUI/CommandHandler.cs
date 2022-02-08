using System;
using System.Collections.Generic;
using System.Linq;
using Banks.Services;
using Banks.Types;
using Spectre.Console;

namespace Banks.CUI
{
    public class CommandHandler : ICommandHandler
    {
        private List<ICommand> _commands;
        public CommandHandler()
        {
        }

        public void UpdateCommands(List<ICommand> commands)
        {
            _commands = commands;
            InitCommandList();
        }

        public void Run()
        {
            while (true)
            {
                try
                {
                    CommandProcessing();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void InitCommandList()
        {
            Grid grid = new Grid()
                .AddColumn(new GridColumn().NoWrap().PadRight(4))
                .AddColumn();

            foreach (ICommand command in _commands)
            {
                grid.AddRow($"[{StylesInfo.MainColor}]{command.Name}[/]", $"[{StylesInfo.SecondaryColor}]{command.Description}[/]");
            }

            grid.AddRow($"[{StylesInfo.MainColor}]/help[/]", $"[{StylesInfo.SecondaryColor}]Shows this menu.[/]");

            AnsiConsole.Write(
                new Panel(grid)
                    .Header(new PanelHeader("Commands list").Centered()).BorderStyle(StylesInfo.SecondaryColor).Expand());
        }

        private void Execute(string input)
        {
            try
            {
                string[] format = input.Split(' ', 2);
                string commandName = format[0];
                string passedArgument = format[1];

                ICommand command = _commands.FirstOrDefault(item => item.Name == commandName && item.IsParameterAvailable);
                if (command == null)
                {
                    AnsiConsole.Write(new Rule($"[{StylesInfo.DangerColor}]Invalid command provided.[/]")
                        .RuleStyle(StylesInfo.NotificationWrapperColor)
                        .LeftAligned());
                }
                else
                {
                    command.SetArgument(passedArgument);
                    command.Execute();
                }
            }
            catch
            {
                ICommand command = _commands.FirstOrDefault(item => item.Name == input && !item.IsParameterAvailable);
                if (command == null)
                {
                    AnsiConsole.Write(new Rule($"[{StylesInfo.DangerColor}]Invalid command provided.[/]")
                        .RuleStyle(StylesInfo.NotificationWrapperColor)
                        .LeftAligned());
                }
                else
                {
                    command.Execute();
                }
            }
        }

        private void CommandProcessing()
        {
            string userInput = Console.ReadLine();
            if (userInput == "/help")
                InitCommandList();
            else
                Execute(userInput);
        }
    }
}