using Cosmos.System;
using SeaLeopard.System.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Console = System.Console;

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

        public string Mode { get; set; }
        public Dictionary<string,Func<string>> InputAction { get; set; }
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
            Commands.Add("readmode", new ReadMode.Instance());

            InputAction = new Dictionary<string, Func<string>>();

            InputAction.Add("readkey", Read);
            InputAction.Add("osi", ReadOSI); 
            InputAction.Add("kmread", KMRead);
            Mode = "readkey";
        }
        public void RunCommand(string name, string[] Args = null)
        {
            Command command = Commands[name].Create(Args);
            int commandResult = command.Run();
        }

        public void UpdateScreen(bool info = false)
        {
            if(info)
            {
                var curp = Console.GetCursorPosition();
                Console.SetCursorPosition(71, 0);
                Console.Write($"{DateTime.Now.ToShortTimeString()}");
                Console.SetCursorPosition(curp.Left, curp.Top);
            }
            else
            {
                Console.Clear();
                Console.WriteLine($"   User: {new String(' ', 15)}  IP: 255.255.255.255  Date: {DateTime.Now.ToShortDateString()}  Time: {DateTime.Now.ToShortTimeString()}");
                Console.WriteLine($"===============================================================================");
                List<string> lines = Lines.Skip(Math.Max(0, Lines.Count() - 21)).ToList();
                lines.ForEach(line => Console.WriteLine(line));
                if (InputMode)
                {
                    Console.Write("> ");
                }
            }
            
        }

        public void Write(string text)
        {
            Lines.Add(text);
            UpdateScreen();
        }
        public bool ReadKeyboardKey(out ConsoleKey Key,out char KeyChar,out ConsoleModifiers Modifiers)
        {
            if (KeyboardManager.TryReadKey(out KeyEvent result))
            {
                Modifiers = result.Modifiers;
                KeyChar = result.KeyChar;
                Key = result.Key.ToConsoleKey();
                return true;
            }
            else
            {
                Key = ConsoleKey.A;
                KeyChar = 'A';
                Modifiers = ConsoleModifiers.Shift;
                return false;
            }
        }
        public string ReadOSI()
        {
            try
            {
                Stream input = Console.OpenStandardInput();
                byte[] bytes = new byte[50];
                int outputLength = input.Read(bytes, 0, 50);
                char[] chars = Encoding.UTF8.GetChars(bytes, 0, outputLength);
                return new string(chars);
            }
            catch (Exception e)
            {
                Write($"OSI Error: {e}");
            }
            return "";
        }
        public string KMRead()
        {
            
            try {
                bool getkeys = true;
                string data = "";

                while (getkeys)
                {
                    if (ReadKeyboardKey(out ConsoleKey key,out char KeyChar,out ConsoleModifiers Modifiers))
                    {
                        if (key != ConsoleKey.Enter)
                        {
                            if (key == ConsoleKey.UpArrow)
                            {
                                historyidx++;
                                if (historyidx > History.Count - 1)
                                {
                                    historyidx--;
                                }
                                data = History[historyidx];
                            }
                            if (key == ConsoleKey.DownArrow)
                            {
                                historyidx--;
                                if (historyidx < History.Count - 1)
                                {
                                    historyidx++;
                                }
                                data = History[historyidx];
                            }
                            if (key == ConsoleKey.Backspace)
                            {
                                data = data.Remove(data.Length - 1, 1);
                            }
                            else
                            {
                                data = data + KeyChar;
                            }


                            UpdateScreen();
                            Console.Write(data);
                        }
                        else
                        {
                            getkeys = false;
                        }
                    }
                    Thread.Sleep(10);
                    UpdateScreen(true);
                }
                return data;
            } 
            catch(Exception e)
            {
                Write($"KMRead Error: {e}");
            }
            return "";
        }
        public string Read()
        {
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
                string input = KMRead();

                    //InputAction[Mode].Invoke();
                Write($" > {input}");
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

        public override void OnChange()
        {
            SeaLeopardManager.terminal = this;
        }
    }
}
