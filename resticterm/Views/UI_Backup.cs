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
        Label current;
        ProgressBar pBar;

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

            // Header
            var header = new Label()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 3
            };
            header.Text = "Path to repository : " + Program.dataManager.config.RepoPath + "\n";
            header.Text += "Sources to backup : " + Program.dataManager.config.SourcesBackupPath + "\n";
            win.Add(header);

            // Information
            info = new Label()
            {
                X = 0,
                Y = Pos.Bottom(header),
                Width = Dim.Fill(),
                Height = Dim.Fill()-2
            };
            info.Text = "";
            win.Add(info);

            // Current
            current = new Label()
            {
                X = 0,
                Y = Pos.Bottom(info),
                Width = Dim.Fill(),
                Height = 1,
                TextAlignment = TextAlignment.Centered
            };
            win.Add(current);

            // Progressbar
            pBar = new ProgressBar()
            {
                X = 0,

                Y = Pos.Bottom(current),
                Width = Dim.Fill(),
                Height = 1,
                Fraction = 1                
            };
            win.Add(pBar);

            Application.Run(ntop);

        }

        void StartBackup()
        {
            Program.restic.Progress += Restic_Progress;
            info.Text = "";
            Program.restic.Backup();
            Program.restic.Progress -= Restic_Progress;
        }

        private void Restic_Progress(string message, float percent)
        {
            if (percent >= 0)
            {
                pBar.Fraction = percent;
                current.Text =  message.Replace("\r", "");
            }
            else
            {
                message = info.Text.ToString() + message + "\n";
                info.Text = message.Replace("\r", "");
            }
            Application.Refresh();
        }
    }
}
