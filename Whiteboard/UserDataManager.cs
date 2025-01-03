using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Whiteboard.Models;

namespace Whiteboard;

public static class UserDataManager
{
    //TODO: standardize this so custom modules can store their own data 
    static string appRoamingPath;

    static List<ModuleInfo> modules;
    public static List<ModuleInfo> Modules => modules;

    static UserDataManager()
    {
        string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        appRoamingPath = Path.Combine(roaming, "Whiteboard");

        Directory.CreateDirectory(appRoamingPath);

        if (File.Exists(appRoamingPath + "/modules.json"))
        {
            string json = File.ReadAllText(appRoamingPath + "/modules.json");
            modules = JsonConvert.DeserializeObject<List<ModuleInfo>>(json)!;
        }
        else
            modules = new();
    }

    public static void AddModule(ModuleInfo moduleInfo)
    {
        modules.Add(moduleInfo);
        SaveModules();
    }

    public static void RemoveModule(ModuleInfo moduleInfo)
    {
        modules.Remove(moduleInfo);
        SaveModules();
    }

    static void SaveModules()
    {
        string json = JsonConvert.SerializeObject(modules);
        File.WriteAllText(appRoamingPath + "/modules.json", json);
    }   
}
