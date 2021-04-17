using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    class UI_Browse
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
                X = 0,
                Y = 0,
                Width = Dim.Fill() ,
                Height = Dim.Fill(),
                ReadOnly = true       
            };
            info.Text = "";
            win.Add(info);

            Application.Run(ntop);
        }

        void Test()
        {
            var snapshots = Program.restic.GetSnapshots("latest");

            var files = Program.restic.GetFilesFromSnapshots(snapshots[0].short_id);


            //var ret = Program.restic._run.Start("ls latest --json ");
            //info.Text += ret;
            //ret = Program.restic._run.Start("snapshots --json");
            //info.Text += ret;
        }
    }
}
