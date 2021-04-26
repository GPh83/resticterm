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

            // Information
            info = new Label()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 2
            };
            info.Text = "";
            win.Add(info);

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
