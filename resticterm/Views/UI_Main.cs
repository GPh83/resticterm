using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using resticterm.Restic;

namespace resticterm.Views
{
    /// <summary>
    /// Main screen with Menu
    /// </summary>
    public class UI_Main
    {
        public void Create()
        {
            // Windows
            var win = new Window("Restic Terminal V" + Assembly.GetExecutingAssembly().GetName().Version.ToString())
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            Application.Top.Add(win);

            // Information
            var info = new TextView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                ReadOnly = true
            };

            if (String.IsNullOrWhiteSpace(Program.dataManager.config.EncryptedRepoPassword) || String.IsNullOrWhiteSpace(Program.dataManager.config.RepoPath))
            {
                // Bad parameters
                info.Text = "Repository and password undefined\nUse Setup\n";
            }
            else
            {
                info.Text = Program.restic.Summary();
            }
                win.Add(info);

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Backup", ShowBackup),
                new StatusItem(Key.F2, "~F2~ Restore", null),
                new StatusItem(Key.F3, "~F3~ Info", ShowInfo),
                new StatusItem(Key.F8, "~F8~ Setup", ShowSetup),
                new StatusItem(Key.F10, "~F10~ Quit", () => { Application.RequestStop(); })
            });
            Application.Top.Add(statusBar);

        }

        void ShowSetup()
        {
            var setup = new Views.UI_Setup();
            setup.Create();
        }
        void ShowInfo()
        {
            var info = new Views.UI_Info();
            info.Create();
        }
        void ShowBackup()
        {
            var bak = new Views.UI_Backup();
            bak.Create();
        }


    }
}
