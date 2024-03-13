using SeaLeopard.System.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaLeopard.System
{
    public abstract class Command : IDisposable
    {
        public abstract class Instance
        {
            public abstract Command Create(string[] args);
        }

        public abstract int Run();

        public void Dispose()
        {

        }
        public abstract string Name
        {
            get; set;
        }
        public abstract string[] ValidArgs { get; set; }
        public abstract string[] Args { get; set; }
        public abstract List<string> Errors { get; set; }

    }


    public class Terminal : App
    {
        public class Instance : App.Instance
        {
            public override App Create(string[] args)
            {
                return new Terminal(args[0].ToString());
            }
        }

        public bool InputMode = true;
        public List<string> Lines { get; set; }
        public List<string> History { get; set; }
        public override string Name {  get; set; }
        public Dictionary<string, Command.Instance> Commands { get; set; }
        public int historyidx = -1;
        public Terminal(string name) { 
            Name = name;
            Lines = new List<string>();

            //create load from user profile if logged in
            History = new List<string>();

            Commands = new Dictionary<string,Command.Instance>();
            LoadCommands();
        }

        public void LoadCommands()
        {
            Commands = new Dictionary<string, Command.Instance>();

            Commands.Add("process",new Process.Instance());
            Commands.Add("motd", new Motd.Instance());
        }
        public void RunCommand(string name, string[] Args = null)
        {
            Command command = Commands[name].Create(Args);
            int commandResult = command.Run();
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
            try
            {
                string input = Read();
                //Write($" > {input}");
                History.Add(input);
                string[] Args = input.Split(' ');
                if (Commands.ContainsKey(Args[0].ToLower()))
                {
                    Command.Instance commandInstance = Commands[Args[0].ToLower()];
                    Command command = commandInstance.Create(Args);
                    if(command.Errors.Count > 0)
                    {
                        command.Errors.ForEach(error =>
                        {
                            SeaLeopardManager.terminal.Write($"{error}");
                            
                        });
                        UpdateScreen();
                    }
                    else
                    {
                        command.Run();
                        SeaLeopardManager.terminal.Write($"Debug: ran command");
                        UpdateScreen();
                    }
                    
                }
                else
                {
                    SeaLeopardManager.terminal.Write($"Invalid command {Args[0]}");
                }
                
            }
            catch(Exception ex)
            {
                Write($"Critical Error: {ex}");
            }
            UpdateScreen();

        }

        
    }
}
