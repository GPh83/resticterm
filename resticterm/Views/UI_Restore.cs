﻿using NStack;
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
        Label info;
        Label current;
        ProgressBar pBar;
        String _snapshotID, _target, _filenameToRestore;


        /// <summary>
        /// Design and show View
        /// </summary>
        public void ShowModal(String snapshotID, String target, String filenameToRestore)
        {
            var ntop = new Toplevel();

            // Save parameters
            _snapshotID = snapshotID;
            _target = target;
            _filenameToRestore = filenameToRestore;

            // StatusBar
            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.Enter, "~Enter~ Restore", StartRestore),
                new StatusItem(Key.Esc, "~Esc~ Return", () => { Application.RequestStop(); })
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            var win = new Window("Restore")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            ntop.Add(win);

            // Header
            var header = new Label()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 3
            };
            var msg = "Path to repository : " + Program.dataManager.config.RepoPath + "\n";
            msg += "Restore from : " + _filenameToRestore + "\n";
            msg += "to : " + _target + "\n";
            header.Text = msg.Replace("\r", "");
            win.Add(header);

            // Information
            info = new Label()
            {
                X = 0,
                Y = Pos.Bottom(header),
                Width = Dim.Fill(),
                Height = Dim.Fill() - 4
            };
            info.Text = "";
            win.Add(info);

            // Current
            current = new Label()
            {
                X = 0,
                Y = Pos.Bottom(info),
                Width = Dim.Fill(),
                Height = 3,
                TextAlignment = TextAlignment.Centered
            };
            win.Add(current);

            // Progressbar
            pBar = new ProgressBar()
            {
                X = 0,

                Y = Pos.Bottom(current),
                Width = Dim.Fill(),
                Height = 1,
                Fraction = 1
            };
            win.Add(pBar);
            pBar.Fraction = 0;

            Application.Run(ntop);

        }

        void StartRestore()
        {
            Program.restic.Progress += Restic_Progress;
            info.Text = "";
            Program.restic.Restore(_snapshotID, _target, _filenameToRestore);
            Program.restic.Progress -= Restic_Progress;
        }

        private void Restic_Progress(string message, float percent)
        {
            if (percent >= 0)
            {
                pBar.Fraction = percent;
                current.Text = message.Replace("\r", "");
            }
            else
            {
                message = info.Text.ToString() + message + "\n";
                info.Text = message.Replace("\r", "");
            }
            Application.Refresh();
        }
    }
}
