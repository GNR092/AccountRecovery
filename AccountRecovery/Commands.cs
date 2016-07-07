using System;
using TShockAPI;
using TShockAPI.DB;

namespace AccountRecovery
{
    class Commands
    {
        public static void EmailUser(CommandArgs args)
        {
            if (args.Parameters.Count != 1)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /email <email>");
                return;
            }
            string email = args.Parameters[0];
            if (Utilities.IsEmailInUse(args.Player.User.ID, email) > 0)
            {
                args.Player.SendErrorMessage("The email is already in use by another account.");
                return;
            }
            else if(Utilities.IsValidEmail(email))
            {
                Utilities.AddEmail(args.Player.User.ID, email);
                args.Player.SendSuccessMessage("Your email has been updated successfully.");
                TShock.Log.ConsoleInfo("{0} has updated their email succesfully.", args.Player.User.Name);
            }
            else
                args.Player.SendErrorMessage("Invalid e-mail address.");
        }

        public static void RecoverPassword(CommandArgs args)
        {
            if (args.Parameters.Count != 2)
            {
                args.Player.SendErrorMessage("Invalid syntax! Proper syntax: /recover <account> <email>");
                return;
            }
            var iCD = AccountRecovery.CommandCooldown;
            if (iCD != null && iCD.ContainsKey(args.Player.Index) && iCD[args.Player.Index] > DateTime.UtcNow)
            {
                args.Player.SendErrorMessage("You must wait {0} minute(s) before sending again.", (int)(iCD[args.Player.Index] - DateTime.UtcNow).TotalMinutes);
                return;
            }
            User user = TShock.Users.GetUserByName(args.Parameters[0]);
            if (user == null)
            {
                args.Player.SendErrorMessage("The account provided does not match our records.");
                return;
            }
            if (Utilities.IsValidEmail(args.Parameters[1]))
            {
                if (Utilities.GetEmailByID(user.ID) == args.Parameters[1])
                {
                    Utilities.SendEmail(args.Player, args.Parameters[1], user);
                    iCD.Add(args.Player.Index, DateTime.UtcNow.AddMinutes(5));
                }
                else
                    args.Player.SendErrorMessage("The account/email does not match our records.");
            }
            else
                args.Player.SendErrorMessage("Invalid e-mail address.");
        }
    }
}
