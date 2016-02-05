using System;
using System.Linq;
using System.Reflection;
using MySql.Data.MySqlClient;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;


namespace AccountRecovery
{
    [ApiVersion(1, 22)]
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
            get { return "Allow recovery of passwords via email."; }
        }

        public static ConfigFile AccountRecoveryConfig { get; set; }

        public AccountRecovery(Main game) : base(game)
        {
            AccountRecoveryConfig = new ConfigFile();
            Order = 10;
        }

        public override void Initialize()
        {
            ConfigFile.SetupConfig();

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            GetDataHandlers.InitGetDataHandler();
        }

        private void OnInitialize(EventArgs args)
        {
            TShock.DB.Query("CREATE TABLE IF NOT EXISTS `Emails`(`ID` INT, `Email` TEXT, PRIMARY KEY(`ID`), FOREIGN KEY(`ID`) REFERENCES `Users`(`ID`) ON UPDATE CASCADE ON DELETE CASCADE)");
            /*SqlTableCreator sqlcreator = new SqlTableCreator(TShock.DB,
            TShock.DB.GetSqlType() == SqlType.Sqlite ? (IQueryBuilder)new SqliteQueryCreator() : new MysqlQueryCreator());
            sqlcreator.EnsureTableStructure(new SqlTable("Emails",
            new SqlColumn("ID", MySqlDbType.Int32) { Primary = true },
            new SqlColumn("Email", MySqlDbType.Text)
            ));*/

            #region Commands
            Action<Command> Add = c =>
            {
                TShockAPI.Commands.ChatCommands.RemoveAll(c2 => c2.Names.Select(s => s.ToLowerInvariant()).Intersect(c.Names.Select(s => s.ToLowerInvariant())).Any());
                TShockAPI.Commands.ChatCommands.Add(c);
            };

            Add(new Command(Permissions.canchangepassword, Commands.EmailUser, "email")
            {
                AllowServer = true,
                HelpText = "Allows a user to change his email or add an email."
            });

            Add(new Command(Permissions.canchat, Commands.RecoverPassword, "recover")
            {
                AllowServer = true,
                HelpText = "Allows a user to request a new password."
            });
            #endregion
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
            }
            base.Dispose(disposing);
        }
    }
}
