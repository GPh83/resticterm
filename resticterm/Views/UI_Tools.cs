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

        public void ShowModal()
        {
            var ntop = new Toplevel();

            // Menu
            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Check repository", CheckRepo),
                new StatusItem(Key.F2, "~F2~ Purge repository", PurgeRepo),
                new StatusItem(Key.F3, "~F3~ Create repository", CreateRepo),
                new StatusItem(Key.F4, "~F4~ Unlock repository", UnlockRepo),
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
            var ms= "Check : Check the repository for errors\n";
            ms += "Purge : Remove snapshots from the repository and remove unneeded data from the repository\n";
            ms += "Create : Initialize a new repository\n";
            ms += "Unlock : Unlock locks other processes created\n";
            info.Text=ms.Replace("\r","");
            scrollView.Add(info);

            Application.Run(ntop);

        }

        void CheckRepo()
        {
            info.Text = Program.restic.Check();
        }
        void PurgeRepo()
        {
            Application.RequestStop();
        }
        void CreateRepo()
        {
            info.Text = Program.restic.Create();

        }
        void UnlockRepo()
        {
            info.Text = Program.restic.Unlock();
        }
    }
}
