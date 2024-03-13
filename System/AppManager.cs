using System;
using System.Collections.Generic;
using System.Linq;

namespace SeaLeopard.System
{
    public abstract class App
    {
        public abstract class Instance
        {
            public abstract App Create(object[] args);
        }
        public abstract void Start();
        public abstract void Update();
    }
    public class AppManager
    {
        public Dictionary<string,App> apps;
        public Dictionary<string, App.Instance> AppList;
        public int CurrentApp;
        public AppManager()
        {
            apps = new Dictionary<string, App>();
            AppList = new Dictionary<string, App.Instance>();
            AppList.Add("Terminal", new Terminal.Instance()) ;

            CurrentApp = 0;
            StartApp("Terminal", new object[] { "SeaLeopard"} , "SeaLeopard Terminal");
        }
        public App StartApp(App AppInstance, string name)
        {
            apps.Add(name, AppInstance);
            return AppInstance;
        }
        public App StartApp(string AppName, object[] args, string name)
        {
            if(AppList.ContainsKey(AppName))
            {
                apps.Add(name, AppList[AppName].Create(args));
                CurrentApp = apps.Count - 1;
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
    }
}
