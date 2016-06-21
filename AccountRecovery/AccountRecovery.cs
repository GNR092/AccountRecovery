using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;
using TShockAPI.Hooks;

namespace AccountRecovery
{
    [ApiVersion(1, 23)]
    public class AccountRecovery : TerrariaPlugin
    {
        public override Version Version
        {
            get { return Assembly.GetExecutingAssembly().GetName().Version; }
        }
        public override string Name
        {
            get { return "Account Recovery"; }
        }
        public override string Author
        {
            get { return "Marcus101RR"; }
        }
        public override string Description
        {
            get { return "Users can add emails, and recovery account passwords."; }
        }

        public static Config Config { get; set; }

        public static Dictionary<int, DateTime> CommandCooldown = new Dictionary<int, DateTime>();

        public AccountRecovery(Main game) : base(game)
        {
            Order = 10;
        }

        public override void Initialize()
        {
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            GetDataHandlers.InitGetDataHandler();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }

        private void OnInitialize(EventArgs args)
        {
            string path = Path.Combine(TShock.SavePath, "AccountRecovery.json");
            Config = Config.Read(path);
            if (!File.Exists(path))
            {
                Config.Write(path);
            }

            if (TShock.DB.GetSqlType() == SqlType.Sqlite)
                TShock.DB.Query("CREATE TABLE IF NOT EXISTS Emails (ID INTEGER PRIMARY KEY AUTOINCREMENT REFERENCES Users(ID) ON UPDATE CASCADE ON DELETE CASCADE, Email TEXT)");
            else
                TShock.DB.Query("CREATE TABLE IF NOT EXISTS Emails (ID INTEGER PRIMARY KEY AUTO_INCREMENT REFERENCES Users(ID) ON UPDATE CASCADE ON DELETE CASCADE, Email TEXT)");

            /*SqlTableCreator sqlcreator = new SqlTableCreator(TShock.DB,
            TShock.DB.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            sqlcreator.EnsureTableStructure(new SqlTable("Emails",
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
            new SqlColumn("Email", MySqlDbType.Text)
            ));*/

            #region Commands
            Action <Command> Add = c =>
            {
                TShockAPI.Commands.ChatCommands.RemoveAll(c2 => c2.Names.Select(s => s.ToLowerInvariant()).Intersect(c.Names.Select(s => s.ToLowerInvariant())).Any());
                TShockAPI.Commands.ChatCommands.Add(c);
            };

            Add(new Command(Permissions.addemail, Commands.EmailUser, "email")
            {
                AllowServer = true,
                HelpText = "Allows a user to change his email or add an email."
            });

            Add(new Command(Permissions.recoveraccount, Commands.RecoverPassword, "recover")
            {
                AllowServer = true,
                HelpText = "Allows a user to request a new password."
            });
            #endregion
        }

        public void OnReload(ReloadEventArgs args)
        {
            string path = Path.Combine(TShock.SavePath, "AccountRecovery.json");
            Config = Config.Read(path);
            if (!File.Exists(path))
            {
                Config.Write(path);
            }
            args.Player.SendSuccessMessage("[Account Recovery] Reloaded configuration file and data!");
        }
    }
}
