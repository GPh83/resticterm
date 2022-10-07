using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    class UI_Tools
    {

        Label info;
        Toplevel ntop;

        public void ShowModal()
        {
            ntop = new Toplevel();

            // Menu
            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Check repo", CheckRepo),
                new StatusItem(Key.F2, "~F2~ Purge repo", PurgeRepo),
                new StatusItem(Key.F3, "~F3~ Create repo", CreateRepo),
                new StatusItem(Key.F4, "~F4~ Unlock repo", UnlockRepo),
                new StatusItem(Key.F5, "~F5~ Upgrade repo to V2", UpgradeRepo),
                new StatusItem(Key.Esc, "~Esc~ Return", () => { Application.RequestStop(); })
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;
            
            // Windows
            var win = new Window("Tools")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            ntop.Add(win);

            // Scroll
            var rect = Application.Top.Frame;
            rect.Width -= 2;
            rect.Height -= 2;
            rect.X = 0;
            rect.Y = 0;
            var scrollView = new ScrollView(rect)
            {
                AutoHideScrollBars = false,
                ShowHorizontalScrollIndicator = false,
                ShowVerticalScrollIndicator = true,
                ContentSize = new Size(rect.Width, 1000)
            };
            win.Add(scrollView);

            // Information
            info = new Label()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            var ms = "\nCheck : Check the repository for errors\n";
            ms += "Purge : Remove snapshots from the repository and remove unneeded data from the repository. Keep last "+ Program.dataManager.config.KeepLastSnapshots.ToString()+ " snapshots\n";
            ms += "Create : Initialize a new repository\n";
            ms += "Unlock : Unlock locks other processes created\n";
            ms += "Upgrade : Upgrade repository to V2 (for compression)\n";
            info.Text = ms.Replace("\r", "");
            scrollView.Add(info);

            Application.Run(ntop);

        }

        void CheckRepo()
        {
            DisplayInfo("Check ...");
            info.Text = Program.restic.Check();
        }

        void PurgeRepo()
        {
            DisplayInfo("Purge ...");
            DisplayInfo(Program.restic.Purge());
            AddInfo( Program.restic.Prune());
        }

        void CreateRepo()
        {
            DisplayInfo("Create ...");
            info.Text = Program.restic.Create();

        }
        void UnlockRepo()
        {
            DisplayInfo("Unlock ...");
            info.Text = Program.restic.Unlock();
        }

        void UpgradeRepo()
        {
            DisplayInfo("Upgrading to repository version 2 ...");
            info.Text = Program.restic.UpgradeV2();
        }
        void DisplayInfo(String message)
        {
            info.Text = message;
            Libs.ViewDesign.RefreshView(ntop, info);
        }

        void AddInfo(String message)
        {
            info.Text += "\n" + message + "\n";
            Libs.ViewDesign.RefreshView(ntop, info);
        }

    }
}
