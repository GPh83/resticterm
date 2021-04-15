using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    class UI_Backup
    {
        Label info;

        /// <summary>
        /// Design and show View
        /// </summary>
        public void Create()
        {
            var ntop = new Toplevel();

            // StatusBar
            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Backup", StartBackup),
                new StatusItem(Key.F10, "~F10~ Return", () => { Application.RequestStop(); })
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            var win = new Window("Backup")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            ntop.Add(win);

            // Information
            info = new Label()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            info.Text = "Path to repository : " + Program.dataManager.config.RepoPath + "\n";
            info.Text += "Source to backup : " + Program.dataManager.config.SourcesBackupPath + "\n";
            win.Add(info);

            Application.Run(ntop);

        }

        void StartBackup()
        {
            info.Text = "";
            Program.restic.Progress += Restic_Progress;
            Program.restic.Backup();
            Program.restic.Progress -= Restic_Progress;
        }

        private void Restic_Progress(string message, int percent)
        {
            message = info.Text.ToString() + message + "\n";
            info.Text = message.Replace("\n","");
            Application.Refresh();
        }
    }
}
