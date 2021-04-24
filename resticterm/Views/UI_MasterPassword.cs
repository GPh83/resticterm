using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    class UI_MasterPassword
    {
        TextField _masterPassword;

        public void ShowModal()
        {
            var ntop = new Toplevel();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.Enter, "~Enter~ Valid", SaveSetup),
                new StatusItem(Key.Esc, "~Esc~ Quit", () => { Application.RequestStop(); })
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            var win = new Window("Master Password")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            ntop.Add(win);

            // Information
            var info = new Label("The master password secure repository password\nDon't loose it !\nIf you change master password you need to redefine repository password\n")
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill()-1,
                Height = 4
            };
            ntop.Add(info);

            Libs.ViewDesign.SetField(ntop, ref _masterPassword, "Master password", "", 30, 6);
            _masterPassword.Secret = true;
            Application.Run(ntop);

        }

        void SaveSetup()
        {
            Program.dataManager.config.MasterPassword = _masterPassword.Text.ToString();
            Application.RequestStop();            
        }

    }

}

