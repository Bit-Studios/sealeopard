using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaLeopard.System
{
    public abstract class App: IDisposable
    {
        public abstract class Instance
        {
            public abstract App Create(object[] args);
        }
        public abstract void Start();
        public abstract void Update();

        public void Dispose()
        {
            
        }
        public abstract string Name
        {
            get; set;
        }
    }
    public class AppManager
    {
        public Dictionary<string, App> apps;
        public Dictionary<string, App.Instance> AppList;
        public int CurrentApp;
        public AppManager()
        {
            apps = new Dictionary<string, App>();
            AppList = new Dictionary<string, App.Instance>();
            AppList.Add("Terminal", new Terminal.Instance());
            AppList.Add("Motd", new Motd.Instance());

            CurrentApp = 0;
            StartApp("Terminal", new object[] { "SLT" }, "SLT");
        }
        public App StartApp(App AppInstance, string name)
        {
            apps.Add(name, AppInstance);
            return AppInstance;
        }
        public App StartApp(string AppName, object[] args, string name)
        {
            if (AppList.ContainsKey(AppName))
            {
                apps.Add(name, AppList[AppName].Create(args));
                CurrentApp = apps.Count - 1;
                apps[name].Start();
                return GetApp(name);
            }
            return null;
        }
        public void Update()
        {
            apps.ToList()[CurrentApp].Value.Update();
        }
        public App GetApp(string AppName)
        {
            return apps[AppName];
        }
        public int ChangeApp(string AppName)
        {
            try
            {
                CurrentApp = apps.ToList().FindIndex(app => app.Key == AppName);
                return CurrentApp;
            }
            catch (Exception ex)
            {
                return -1;
            }


        }
        public void StopApp(string appname, bool Silent = false)
        {
            if (CurrentApp == apps.Count - 1)
            {
                CurrentApp = 0;
            }
            apps[appname].Dispose();
            apps.Remove(appname);
            if (!Silent)
            {
                SeaLeopardManager.terminal.Write($"Stopped process {appname}");
            }
            Update();
        }
    }
}
