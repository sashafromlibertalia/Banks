using Banks.CUI;
using Banks.CUI.States;
using Banks.Entities;
using Banks.Types;
using Spectre.Console;

namespace Banks
{
    public class App
    {
        public App()
        {
            AnsiConsole.Write(new Rule($"[b {StylesInfo.MainColor}]Banks system :bank:[/]").RoundedBorder().RuleStyle("grey"));
            AnsiConsole.WriteLine();
        }

        public void Run()
        {
            var centralBank = new CentralBank();
            var context = new Context(new MainState(), centralBank);
            context.ChangeCommands();
        }
    }
}