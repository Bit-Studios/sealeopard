using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaLeopard.System.Commands
{
    public class Motd : Command
    {
        public override string Name { get; set; }
        public override string[] ValidArgs { get; set; }
        public override string[] Args { get; set; }
        public override List<string> Errors { get; set; }

        public class Instance : Command.Instance
        {
            public override Command Create(string[] args = null)
            {
                try
                {
                    args = args[1..];
                }
                catch (Exception e)
                {

                }
                return new Motd(args);
            }
        }
        public Motd(string[] args = null)
        {
            Name = "Motd";
        }

        public override int Run()
        {
            SeaLeopardManager.terminal.Write("Welcome to SeaLeopard");
            SeaLeopardManager.terminal.Write($"-------------------------------------------");
            SeaLeopardManager.terminal.Write($"Time: {DateTime.Now.ToShortTimeString()}");
            SeaLeopardManager.terminal.Write($"-------------------------------------------");
            SeaLeopardManager.terminal.Write($"Attached drives: ");
            SeaLeopardManager.terminal.Write($"IP: ");
            SeaLeopardManager.terminal.Write($"User: ");
            return 0;
        }
    }
}
