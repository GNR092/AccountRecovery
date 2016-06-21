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
    public class Config
    {
        public string ServerEmailAddress = "example@gmail.com";
        public string ServerEmailPassword = "password";
        public string HostSMTPServer = "smtp.gmail.com";
        public int HostPort = 587;
        public string EmailFrom = "noreply@gmail.com";
        public string EmailSubjectLine = "Password Recovery";
        public string EmailBodyLine = "YOUR SERVER NAME Password Recovery\nYou have requested a new password. We have successfully generated a new password for your account.\n\nYour new password: $NEW_PASSWORD";
        public int GeneratedPasswordLength = 6;

        public void Write(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static Config Read(string path)
        {
            return File.Exists(path) ? JsonConvert.DeserializeObject<Config>(File.ReadAllText(path)) : new Config();
        }
    }
}