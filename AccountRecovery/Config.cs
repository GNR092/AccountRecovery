using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TShockAPI;

namespace AccountRecovery
{
    public class ConfigFile
    {
        public string ServerEmailAddress = "example@gmail.com";
        public string ServerEmailPassword = "password";
        public string HostSMTPServer = "smtp.gmail.com";
        public int HostPort = 587;
        public string EmailFrom = "noreply@recovery.com";
        public string EmailSubjectLine = "Password Recovery";
        public string EmailBodyLine = "YOUR SERVER NAME Password Recovery\n You have requested a new password. We have successfully generated a new password for your account. \n\nYour new password: $NEW_PASSWORD";

        public static ConfigFile Read(string path)
        {
            if (!File.Exists(path))
                return new ConfigFile();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        public static ConfigFile Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<ConfigFile>(sr.ReadToEnd());
                if (ConfigRead != null)
                    ConfigRead(cf);
                return cf;
            }
        }

        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }

        public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }

        public static Action<ConfigFile> ConfigRead;

        internal static string ConfigPath { get { return Path.Combine(TShock.SavePath, "AccountRecovery.json"); } }

        public static void SetupConfig()
        {
            try
            {
                if (File.Exists(ConfigPath))
                    AccountRecovery.AccountRecoveryConfig = Read(ConfigPath);
                /* Add all the missing config properties in the json file */

                AccountRecovery.AccountRecoveryConfig.Write(ConfigPath);
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("Config Exception: Error in config file");
                TShock.Log.Error(ex.ToString());
            }
        }
    }
}