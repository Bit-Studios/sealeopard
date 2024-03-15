using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaLeopard.System.Commands
{
    public class ReadMode : Command
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
                return new ReadMode(args);
            }
        }
        public ReadMode(string[] args = null)
        {
            try
            {
                Name = "ReadMode";
                if (args.Length > 0)
                {
                    Args = args;
                }
                else
                {
                    Errors.Add($"requires args");
                }

            }
            catch (Exception e)
            {
                SeaLeopardManager.terminal.Write($"ReadMode command failed to create {e}");
                SeaLeopardManager.terminal.UpdateScreen();
            }
        }

        public override int Run()
        {
            try
            {
                if (SeaLeopardManager.terminal.InputAction.ContainsKey(Args[0].ToLower()))
                {
                    SeaLeopardManager.terminal.Mode = Args[0].ToLower();
                }
            }
            catch (Exception e)
            {
                SeaLeopardManager.terminal.Write($"Invalid Arguments {Args} \nReadMode <Mode>\n{e}");
            }
            return 0;
        }
    }
}
