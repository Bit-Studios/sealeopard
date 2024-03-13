using SeaLeopard.System;
using System;
using Sys = Cosmos.System;


namespace SeaLeopard
{
    public static class SeaLeopardManager
    {
        public static Terminal terminal { get; set; }
        public static AppManager appManager { get; set; }
    }
    public class Kernel : Sys.Kernel
    {
        Terminal terminal => SeaLeopardManager.terminal;
        AppManager appManager => SeaLeopardManager.appManager;
        protected override void BeforeRun()
        {
            SeaLeopardManager.appManager = new AppManager();
            Console.WriteLine("Loading SeaLeopard");
            SeaLeopardManager.terminal = (Terminal)appManager.GetApp("SeaLeopard Terminal");
            terminal.Write("Welcome to SeaLeopard");
        }

        protected override void Run()
        {
            appManager.Update();
        }
    }
}
