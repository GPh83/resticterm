using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using resticterm.Restic;
using System.Diagnostics;
using System.IO;

namespace resticterm.Views
{

    /// <summary>
    /// Main screen with Menu
    /// </summary>
    public class UI_Main
    {
        Label info;
        Window win;

        public void Create()
        {
            // Windows
            win = new Window("Restic Terminal V" + Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.Add(win);
            Application.Top.Ready += () => { MainReady(); };


            // Information
            info = new Label()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            win.Add(info);

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Backup", ShowBackup),
                new StatusItem(Key.F2, "~F2~ Browse/Restore", ShowBrowser),
                new StatusItem(Key.F8, "~F8~ Setup", ShowSetup),
                new StatusItem(Key.F10, "~F10~ Quit", () => { Application.RequestStop(); })
            });
            Application.Top.Add(statusBar);

        }

        public void MainReady()
        {
            // Master password if necessary
            if (Program.dataManager.config.UseMasterPassword)
            {
                var pwd = new Views.UI_MasterPassword();
                pwd.ShowModal();
            }
            
            // Config state
            if (String.IsNullOrWhiteSpace(Program.dataManager.config.EncryptedRepoPassword) || String.IsNullOrWhiteSpace(Program.dataManager.config.RepoPath))
            {
                // Bad parameters
                info.Text = "Repository or password undefined\nUse Setup\n";
            }
            else
            {
                if (Directory.Exists(Program.dataManager.config.RepoPath))
                    DisplayRepoSummary();
                else
                    info.Text = "Bad Repository path\nUse Setup\n";
            }
        }


        void ShowSetup()
        {
            var setup = new Views.UI_Setup();
            setup.ShowModal();
            Program.restic = new Restic.Restic(Program.dataManager.config.RepoPath, Program.dataManager.config.EncryptedRepoPassword);
            DisplayRepoSummary();
        }
        void ShowBrowser()
        {
            var info = new Views.UI_Browse();
            info.ShowModal();
            DisplayRepoSummary();
        }
        void ShowBackup()
        {
            var bak = new Views.UI_Backup();
            bak.ShowModal();
            DisplayRepoSummary();
        }

        void DisplayRepoSummary()
        {
            var str = Program.restic.Summary();
            str += "\n";
            str += "resticterm Copyright(C) 2021 Philippe GRAILLE. This program comes with ABSOLUTELY NO WARRANTY. This is free software, and you are welcome to redistribute it under certain conditions, see GNU GPL V3 : https://www.gnu.org/licenses/\n";
            str += "GitHub : https://github.com/GPh83/resticterm/\n";
            info.Text = str.Replace("\r", "");
        }

    }
}
