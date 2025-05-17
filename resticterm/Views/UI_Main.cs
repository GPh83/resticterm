using resticterm.Models;
using resticterm.Restic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{

    /// <summary>
    /// Main screen with Menu
    /// </summary>
    public class UI_Main
    {
        Label info;
        StatusBar statusBar;
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

            statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Backup", () => { ShowBackup(); }),
                new StatusItem(Key.F2, "~F2~ Browse/Restore", () => {ShowBrowser(); }),
                new StatusItem(Key.F3, "~F3~ Init/Tools", () => {ShowTools(); }),
                new StatusItem(Key.F8, "~F8~ Setup", () => { ShowSetup(); }),
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
            DisplayRepoSummary();
        }


        void ShowSetup(String message = "")
        {
            if (Application.Top.IsCurrentTop)
            {
                var setup = new Views.UI_Setup();
                setup.ShowModal(message);
                DisplayRepoSummary();
            }
        }

        void ShowBrowser()
        {
            if (Application.Top.IsCurrentTop)
            {
                var chk = Program.dataManager.config.CheckValidity();
                if (chk == "")
                {
                    var info = new Views.UI_Browse();
                    info.ShowModal();
                    DisplayRepoSummary();
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Invalid setup, use Setup !\n\n" + chk, "Ok");
                }
            }
        }

        void ShowBackup()
        {
            if (Application.Top.IsCurrentTop)
            {
                var chk = Program.dataManager.config.CheckValidity();
                if (chk == "")
                {
                    var bak = new Views.UI_Backup();
                    bak.ShowModal();
                    DisplayRepoSummary();
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Invalid setup, use Setup !\n\n" + chk, "Ok");
                }
            }
        }

        void ShowTools()
        {
            if (Application.Top.IsCurrentTop)
            {
                var chk = Program.dataManager.config.CheckValidity();
                if (chk == "")
                {
                    var tools = new Views.UI_Tools();
                    tools.ShowModal();
                    DisplayRepoSummary();
                }
                else
                {
                    MessageBox.ErrorQuery("Error", "Invalid setup, use Setup !\n\n" + chk, "Ok");
                }
            }
        }

        void DisplayRepoSummary()
        {
            var chk = Program.dataManager.config.CheckValidity();
            if (!string.IsNullOrEmpty(chk))
            {
                info.Text = "Bad setup !\n\n" + chk;
                ShowSetup(chk);
            }
            else
            {
                info.Text = "Summary in progress ...";
                Libs.ViewDesign.RefreshView(Application.Top, info);

                var str = Program.restic.Summary();
                str += "\n";
                str+= "resticterm latest release : " + GitHubRelease.GetRestictermRelease();
                str += "\n";
                str += "resticterm Copyright(C) 2021-2025 Philippe GRAILLE. This program comes with ABSOLUTELY NO WARRANTY. This is free software, and you are welcome to redistribute it under certain conditions, see GNU GPL V3 : https://www.gnu.org/licenses/\n";
                str += "GitHub : https://github.com/GPh83/resticterm/\n";
                info.Text = str.Replace("\r", "");
                Application.Refresh();
            }
        }
    }
}
