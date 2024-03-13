using Cosmos.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaLeopard.System
{
    public class Motd : App
    {
        public override string Name { get; set; }

        public class Instance : App.Instance
        {
            public override App Create(object[] args)
            {
                return new Motd(args[0].ToString());
            }
        }
        public Motd(string name)
        {
            Name = name;
        }
        public override void Start()
        {
            SeaLeopardManager.appManager.ChangeApp(SeaLeopardManager.terminal.Name);
            SeaLeopardManager.terminal.InputMode = false;
            SeaLeopardManager.terminal.Write("Welcome to SeaLeopard");
            SeaLeopardManager.terminal.Write($"-------------------------------------------");
            SeaLeopardManager.terminal.Write($"Time: {DateTime.Now.ToShortTimeString()}");
            SeaLeopardManager.terminal.Write($"-------------------------------------------");
            SeaLeopardManager.terminal.Write($"Attached drives: ");
            SeaLeopardManager.terminal.Write($"IP: ");
            SeaLeopardManager.terminal.Write($"User: ");
            SeaLeopardManager.terminal.InputMode = true;
            
            SeaLeopardManager.appManager.StopApp(Name);
        }

        public override void Update()
        {
            SeaLeopardManager.terminal.Update();
        }
    }
}
