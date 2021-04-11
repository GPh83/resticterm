using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    class UI_Info
    {
        TextView info;

        public void Create()
        {
            var ntop = new Toplevel();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Test", Test),
                new StatusItem(Key.F10, "~F10~ Return", () => { Application.RequestStop(); })
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            var win = new Window("Informations")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            ntop.Add(win);

            info = new TextView()
            {
                X = 2,
                Y = 2,
                Width = Dim.Fill() - 4,
                Height = Dim.Fill() - 4,
                ReadOnly = true       
            };
            info.Text = "Current state : ";
            win.Add(info);

            Application.Run(ntop);
        }

        void Test()
        {
            //var ret = Restic.Run.Start("stats", "-r G:\\Backup\\repo");
            //info.Text = ret;
        }
    }
}
