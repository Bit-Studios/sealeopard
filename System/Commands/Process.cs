﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaLeopard.System.Commands
{
    public class Process : Command
    {
        public override string Name { get; set; }
        public override string[] ValidArgs { get; set; }
        public override List<string> Errors { get; set; }
        public override string[] Args { get; set; }

        public override int Run()
        {
            try
            {
                switch (Args[0].ToLower())
                {
                    case "start":
                        try
                        {
                            App StartApp = SeaLeopardManager.appManager.StartApp(Args[1],new string[] { Args[2] }, Args[2]);
                            SeaLeopardManager.appManager.ChangeApp(Args[2]);
                            if (StartApp == null)
                            {
                                SeaLeopardManager.terminal.Write($"Invalid App {Args[1]}");
                            }
                        }
                        catch (Exception e)
                        {
                            SeaLeopardManager.terminal.Write($"Invalid Arguments {Args} \nProcess Start <App> <Name (unique)>\nExample: Process Start Terminal Example_Terminal");
                        }
                        break;
                    case "list":
                        try
                        {
                            int idx = 0;
                            SeaLeopardManager.terminal.Write($"Currently running processes");
                            SeaLeopardManager.terminal.Write($"Process Name | App Name");
                            SeaLeopardManager.appManager.apps.ToList().ForEach(app =>
                            {
                                SeaLeopardManager.terminal.Write($"{app.Key} | {app.Value.GetType().Name}");
                                if (idx > 21)
                                {
                                    SeaLeopardManager.terminal.InputMode = false;
                                    SeaLeopardManager.terminal.Write("======== Press enter for more ========");
                                    SeaLeopardManager.terminal.Read();
                                }
                                SeaLeopardManager.terminal.InputMode = true;
                            });
                        }
                        catch (Exception e)
                        {
                            SeaLeopardManager.terminal.Write($"Invalid Arguments {Args} \nProcess List");
                            SeaLeopardManager.terminal.UpdateScreen();
                        }
                        break;
                    case "stop":
                        try
                        {
                            if(Args.Length > 1)
                            {
                                if(bool.TryParse(Args[2], out bool argb))
                                {
                                    SeaLeopardManager.appManager.StopApp(Args[1], Force: argb);
                                }
                                else
                                {
                                    SeaLeopardManager.terminal.Write($"Invalid Arguments {Args} \nProcess Stop <Process Name> ?<force (true | false)>");
                                    SeaLeopardManager.terminal.UpdateScreen();
                                }
                            }
                            else
                            {
                                SeaLeopardManager.appManager.StopApp(Args[1]);
                            }
                        }
                        catch (Exception e)
                        {
                            SeaLeopardManager.terminal.Write($"Invalid Arguments {Args} \nProcess Stop <Process Name>");
                            SeaLeopardManager.terminal.UpdateScreen();
                        }
                        
                        break;
                    case "set":
                        try
                        {
                            SeaLeopardManager.appManager.ChangeApp(Args[1]);
                        }
                        catch (Exception e)
                        {
                            SeaLeopardManager.terminal.Write($"Invalid Arguments {Args} \nProcess Set <Process Name>");
                            SeaLeopardManager.terminal.UpdateScreen();
                        }

                        break;

                    default:
                        SeaLeopardManager.terminal.Write($"Invalid Args {Args} how did you even get this???");
                        SeaLeopardManager.terminal.UpdateScreen();
                        break;
                }
                return 0;
            }
            catch(Exception e)
            {
                SeaLeopardManager.terminal.Write($"Process command failed to run {e}");
                SeaLeopardManager.terminal.UpdateScreen();
                return 1;
            }
        }


        public class Instance : Command.Instance
        {
            public override Command Create(string[] args = null)
            {
                try
                {
                    List<string> tmpargs = args.ToList();
                    tmpargs.RemoveAt(0);
                    args = tmpargs.ToArray();
                }
                catch(Exception e)
                {

                }
                
                return new Process(args);
            }
        }
        public Process(string[] args = null)
        {
            
            try
            {
                Name = "Process";
                Errors = new List<string>();
                ValidArgs = new string[] {
                "start",
                "stop",
                "list",
                "set"
                    };
                if(args.Length > 0)
                {
                    if (ValidArgs.Contains(args[0].ToLower()))
                    {
                        Args = args;
                    }
                    else
                    {
                        Errors.Add($"Invalid argument {args[0]}");
                    }
                }
                else
                {
                    Errors.Add($"requires args");
                }
                
            }
            catch (Exception e)
            {
                SeaLeopardManager.terminal.Write($"Process command failed to create {e}");
                SeaLeopardManager.terminal.UpdateScreen();
            }
            
        }
    }
}
