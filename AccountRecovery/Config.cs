using System.IO;
using Newtonsoft.Json;

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
        public string EmailBodyLine = "YOUR SERVER NAME Password Recovery\n\nHello $USERNAME,\n\nYou have requested a new password from our server. We have successfully generated a new password for your account.\n\nYour new password: $NEW_PASSWORD";
        public int GeneratedPasswordLength = 6;
        public bool UseHTML = false;

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