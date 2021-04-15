using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    class UI_Restore
    {

        public void Create()
        {
            var ntop = new Toplevel();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Backup", SaveSetup),
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

            //Libs.ViewDesign.SetField(ntop, ref _repoPath, "Path to repository", Program.dataManager.config.RepoPath, 30, 3);
            //Libs.ViewDesign.SetField(ntop, ref _repoPassword, "Repository password", Program.dataManager.config.GetRepoPassword(), 30, 4);
            //_repoPassword.Secret = true;
            Application.Run(ntop);

        }

        void SaveSetup()
        {
            //Program.dataManager.config.RepoPath = _repoPath.Text.ToString();
            //Program.dataManager.config.SetRepoPassword(_repoPassword.Text.ToString());
            //Program.dataManager.SaveConfig();
            Application.RequestStop();
        }

    }
}
