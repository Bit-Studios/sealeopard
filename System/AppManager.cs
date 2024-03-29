﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaLeopard.System
{
    public abstract class App: IDisposable
    {
        public abstract class Instance
        {
            public abstract App Create(string[] args);
        }
        public abstract void Start();
        public abstract void Update();
        public abstract void OnChange();
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

            CurrentApp = 0;
            StartApp("Terminal", new string[] { "SLT" }, "SLT");
        }
        public App StartApp(App AppInstance, string name)
        {
            apps.Add(name, AppInstance);
            return AppInstance;
        }
        public App StartApp(string AppName, string[] args, string name)
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
            try
            {
                apps.ToList()[CurrentApp].Value.Update();
            }
            catch (Exception e)
            {
                SeaLeopardManager.terminal.Write($"{e}");
            }
            
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
                apps.ToList()[CurrentApp].Value.OnChange();
                return CurrentApp;
            }
            catch (Exception ex)
            {
                SeaLeopardManager.terminal.Write($"Error: {ex}");
                return -1;
            }


        }
        public void StopApp(string appname, bool Silent = false, bool Force = false)
        {
            if (CurrentApp == apps.Count - 1)
            {
                CurrentApp = 0;
            }
            if(appname == apps.ToList()[CurrentApp].Key)
            {
                CurrentApp = 0;
            }
            if (appname != apps.ToList()[0].Key || Force == true)
            {
                apps[appname].Dispose();
                apps.Remove(appname);
            }
            if (!Silent)
            {
                SeaLeopardManager.terminal.Write($"Stopped process {appname}");
            }
            Update();
        }
    }
}
