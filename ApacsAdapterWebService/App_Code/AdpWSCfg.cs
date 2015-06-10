using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Configuration;
using ApacsAdapter;

public class AdpWSCfg
{
    private Configuration cfg;
    public string GRhost { get; private set; }
    public string GRuser { get; private set; }
    public string GRpassword { get; private set; }
    public string apcLogin { get; private set; }
    public string apcPasswd { get; private set; }


    public AdpWSCfg()
    {
        cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        // Create config if not exists
        if (cfg.AppSettings.Settings.Count == 0)
        {
            createConfig();
        }
        readConfig();
    }

    // Read config file and set property value in config class uses reflection
    private void readConfig()
    {
        this.GRhost = ConfigurationManager.AppSettings["GRhost"];
        this.GRuser = ConfigurationManager.AppSettings["GRuser"];
        this.GRpassword = ConfigurationManager.AppSettings["GRpassword"];
        this.apcLogin = ConfigurationManager.AppSettings["apcLogin"];
        this.apcPasswd = ConfigurationManager.AppSettings["apcPasswd"];
    }

    // Create config file uses default property value 
    private void createConfig()
    {
        // WSO2 Governancy Registry default settings
        //"10.28.65.228"; // PROD SERVER
        cfg.AppSettings.Settings.Add("GRhost", "192.168.0.151");
        cfg.AppSettings.Settings.Add("GRuser", "Apacs");
        cfg.AppSettings.Settings.Add("GRpassword", "Aa1234567");

        // Apacs user\pass settings
        cfg.AppSettings.Settings.Add("apcLogin", "Inst");
        cfg.AppSettings.Settings.Add("apcPasswd", "1945");
    }
}

