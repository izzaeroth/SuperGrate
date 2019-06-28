﻿using System.Xml.Linq;
using System.IO;
using System;
using System.Collections.Generic;

namespace SuperGrate
{
    class Config
    {
        public static Dictionary<string, string> Settings = new Dictionary<string, string>() {
            {"XComment1", @"The UNC or Direct path to the USMT Migration Store E.g: \\ba-share\s$ or .\STORE."},
            {"MigrationStorePath", @".\STORE"},
            {"XComment2", "ScanState.exe & LoadState.exe CLI Parameters: https://docs.microsoft.com/en-us/windows/deployment/usmt/usmt-command-line-syntax "},
            {"ScanStateParameters", "/config:Config_SettingsOnly.xml /i:MigUser.xml /r:3 /o"},
            {"LoadStateParameters", "/config:Config_SettingsOnly.xml /i:MigUser.xml /r:3"}
        };
        public static void GenerateConfig()
        {
            Logger.Warning("Generating new SuperGrate.xml config.");
            XElement root = new XElement("SuperGrate");
            foreach(KeyValuePair<string, string> setting in Settings)
            {
                if(setting.Key.StartsWith("XComment"))
                {
                    root.Add(new XComment(setting.Value));
                }
                else
                {
                    root.Add(new XElement(setting.Key, setting.Value));
                }
            }
            new XDocument(root).Save(@".\SuperGrate.xml");
        }
        public static void LoadConfig()
        {
            if(!File.Exists(@".\SuperGrate.xml"))
            {
                GenerateConfig();
            }
            try
            {
                XDocument config = XDocument.Load(@".\SuperGrate.xml");
                XElement root = config.Element("SuperGrate");
                bool success = true;
                Dictionary<string, string> xmlSettings = new Dictionary<string, string>();
                foreach(KeyValuePair<string, string> setting in Settings)
                {
                    if (!setting.Key.StartsWith("XComment"))
                    {
                        XElement element = root.Element(setting.Key);
                        if (element == null)
                        {
                            success = false;
                            Logger.Warning("SuperGrate.xml is missing: " + setting.Key + "!");
                        }
                        else
                        {
                            xmlSettings[setting.Key] = element.Value;
                        }
                    }
                }
                Settings = xmlSettings;
                if(success)
                {
                    Logger.Success("Config loaded!");
                }
                else
                {
                    Logger.Warning("Config loaded, but is using default values for the missing elements.");
                }
            }
            catch(Exception e)
            {
                Logger.Exception(e, "Error when loading the Super Grate config file! SuperGrate.xml");
            }
        }
    }
}
