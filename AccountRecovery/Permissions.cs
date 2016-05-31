using System.ComponentModel;

namespace AccountRecovery
{
    class Permissions
    {
        [Description("Add Email to Account")]
        public static readonly string addemail = "recovery.user.email";

        [Description("Recover Account from Email")]
        public static readonly string recoveraccount = "recovery.user.recover";
    }
}
