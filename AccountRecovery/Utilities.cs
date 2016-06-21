using System;
using System.Net.Mail;
using System.Text;
using TShockAPI;
using TShockAPI.DB;

namespace AccountRecovery
{
    class Utilities
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var mail = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static void SendEmail(string email, TSPlayer player)
        {
            MailMessage mail = new MailMessage(AccountRecovery.Config.EmailFrom, email);
            SmtpClient client = new SmtpClient();
            client.Timeout = 15000;
            client.Host = AccountRecovery.Config.HostSMTPServer;
            client.Port = AccountRecovery.Config.HostPort;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential(AccountRecovery.Config.ServerEmailAddress, AccountRecovery.Config.ServerEmailPassword);
            client.EnableSsl = true;
            //client.ServicePoint.MaxIdleTime = 1;
            mail.Subject = AccountRecovery.Config.EmailSubjectLine;
            mail.Body = AccountRecovery.Config.EmailBodyLine;
            mail.IsBodyHtml = false;

            string passwordGenerated = GeneratePassword(AccountRecovery.Config.GeneratedPasswordLength);
            TShock.Users.SetUserPassword(player.User, passwordGenerated);
            TShock.Log.ConsoleInfo("{0} has requested a new password succesfully.", player.User.Name);
            mail.Body = string.Format(AccountRecovery.Config.EmailBodyLine.Replace("$NEW_PASSWORD", passwordGenerated), passwordGenerated);

            client.Send(mail);
            client.Dispose();
            player.SendSuccessMessage("Your password has been sent to your email.");
        }

        public static string GetEmailByID(int accountID)
        {
            try
            {
                using (var reader = TShock.DB.QueryReader("SELECT * FROM Emails WHERE ID = @0", accountID))
                {
                    if (reader.Read())
                    {
                        return reader.Get<string>("Email");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("SQL returned an error: " + ex);
            }
            return null;
        }

        public static int IsEmailInUse(int accountID, string email)
        {
            try
            {
                using (var reader = TShock.DB.QueryReader("SELECT * FROM Emails WHERE ID != @0 AND Email = @1", accountID, email))
                {
                    if (reader.Read())
                    {
                        return reader.Get<int>("ID");
                    }
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError("SQL returned an error: " + ex);
            }
            return -1;
        }

        public static bool AddEmail(int accountID, string email = "", bool exceptions = false)
        {
            try
            {
                if (GetEmailByID(accountID) != null)
                {
                    return TShock.DB.Query("UPDATE Emails SET ID = @0, Email = @1 WHERE ID = @2", accountID, email, accountID) == 1;
                }
                else
                {
                    return TShock.DB.Query("INSERT INTO Emails (ID, Email) VALUES (@0, @1);", accountID, email) != 0;
                }
            }
            catch (Exception ex)
            {
                if (exceptions)
                    throw ex;
                TShock.Log.Error(ex.ToString());
            }
            return false;
        }

        public static string GeneratePassword(int length)
        {
            Random random = new Random();
            string characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder result = new StringBuilder(length);
            for (int i = 0; i < length; i++)
            {
                result.Append(characters[random.Next(characters.Length)]);
            }
            return result.ToString();
        }
    }
}
