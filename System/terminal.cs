using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;

namespace SeaLeopard.System
{
    public class Terminal : App
    {
        public class Instance : App.Instance
        {
            public override App Create(object[] args)
            {
                return new Terminal(args[0].ToString());
            }
        }

        public bool InputMode = true;
        public List<string> Lines { get; set; }
        public List<string> History { get; set; }
        public override string Name {  get; set; }
        public int historyidx = -1;
        public Terminal(string name) { 
            Name = name;
            Lines = new List<string>();

            //create load from user profile if logged in
            History = new List<string>();
        }

        public void UpdateScreen()
        {
            Console.Clear();
            List<string> lines = Lines.Skip(Math.Max(0, Lines.Count() - 24)).ToList();
            lines.ForEach(line => Console.WriteLine(line));
            if (InputMode)
            {
                Console.Write("> ");
            }
        }

        public void Write(string text)
        {
            Lines.Add(text);
            UpdateScreen();
        }
        public string Read(string label = "")
        {
            Console.Write(label);
            ConsoleKeyInfo key = Console.ReadKey(false);
            string data = "";
            while (key.Key != ConsoleKey.Enter)
            {
                
                if (key.Key == ConsoleKey.UpArrow)
                {
                    historyidx++;
                    if(historyidx > History.Count - 1)
                    {
                        historyidx--;
                    }
                    data = History[historyidx];
                }
                if (key.Key == ConsoleKey.DownArrow)
                {
                    historyidx--;
                    if (historyidx < History.Count - 1)
                    {
                        historyidx++;
                    }
                    data = History[historyidx];
                }
                if(key.Key == ConsoleKey.Backspace)
                {
                    data = data.Remove(data.Length - 1, 1);
                }
                else
                {
                    data = data + key.KeyChar;
                }
                
                
                UpdateScreen();
                Console.Write(data);
                key = Console.ReadKey(false);
                
            }
            return data;

        }

        public override void Start()
        {
            
        }

        public override void Update()
        {
            string input = Read();
            Write($" > {input}");
            History.Add(input);
            string[] Args = input.Split(' ');
            switch (Args[0].ToLower())
            {
                case "start":
                    try
                    {
                        App StartApp = SeaLeopardManager.appManager.StartApp(Args[1], Args, Args[2]);
                        if (StartApp == null)
                        {
                            Write($"Invalid App {Args[1]}");
                        }
                    }
                    catch(Exception e)
                    {
                        Write($"Invalid Arguments {Args} \nStart <App> <Name (unique)>\nExample: Start Terminal Example_Terminal");
                    }
                    break;
                case "process":
                    try
                    {
                        switch (Args[1])
                        {
                            case "List":
                                int idx = 0;
                                Write($"Currently running processes");
                                Write($"Process Name | App Name");
                                SeaLeopardManager.appManager.apps.ToList().ForEach(app =>
                                {
                                    Write($"{app.Key} | {app.Value.GetType().Name}");
                                    if(idx > 23)
                                    {
                                        InputMode = false;
                                        Read("======== Press enter for more ========");
                                    }
                                    InputMode = true;
                                });
                                break;
                            case "Stop":
                                SeaLeopardManager.appManager.StopApp(Args[2]);
                                break;
                            default:

                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Write($"Invalid Arguments {Args} \nProcess List\nProcess Stop <Name>");
                    }
                    break;
                default:
                    Write($"Invalid command {Args[0]}");
                    break;
            }
            UpdateScreen();
        }

        
    }
}
