using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    public class DialogBackupError
    {
        public static  bool AskQuitOnError(String message)
        {
            return MessageBox.ErrorQuery("Backup error", "An error occurred during backup.\n" + message + "\n\nWhat to do ?\n", "Abort","Continue") == 0;
        }

    }
}
