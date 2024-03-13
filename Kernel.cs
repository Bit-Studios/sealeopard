using Cosmos.System.Graphics;
using SeaLeopard.System;
using System;
using System.Net.Http.Headers;
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
            SeaLeopardManager.terminal = (Terminal)appManager.GetApp("SLT");
            appManager.StartApp("Motd", new object[] { "SLmotd" }, "SLmotd");
        }

        protected override void Run()
        {
            appManager.Update();
        }
    }
}
