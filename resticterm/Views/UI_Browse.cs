using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using static Terminal.Gui.TableView;

namespace resticterm.Views
{
    class UI_Browse
    {
        Label info;
        TableView tv;

        public void ShowModal()
        {
            var ntop = new Toplevel();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.Enter, "~Enter~ Enter", onSelect),
                new StatusItem(Key.Esc, "~Esc~ Return", Quit)
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            var win = new Window("Browse")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            ntop.Add(win);

            info = new Label()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = 1,
                TextAlignment = TextAlignment.Centered
            };
            info.Text = "Select a snapshot to see files";
            win.Add(info);

            tv = new TableView()
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true
            };
            win.Add(tv);
            ShowSnapshots();
            tv.CellActivated += onCellActivated;

            Application.Run(ntop);
        }

        void Quit()
        {
            tv.CellActivated -= onCellActivated;
            Application.RequestStop();
        }


        void onCellActivated(CellActivatedEventArgs cellEvt)
        {
            if (cellEvt.Row >= 0)
            {
                var id = cellEvt.Table.Rows[cellEvt.Row][0];
                var files = new Views.UI_Files();
                files.ShowModal(id.ToString());
            }
        }

        void onSelect()
        {
            if (tv.SelectedRow >= 0)
            {
                var id = tv.Table.Rows[tv.SelectedRow][0];
                var files = new Views.UI_Files();
                files.ShowModal(id.ToString());
            }
        }


        void ShowSnapshots()
        {
            var snapshots = Program.restic.GetSnapshots();

            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Time");
            dt.Columns.Add("Hostname");
            dt.Columns.Add("Path");

            foreach (Models.SnapshotItem s in snapshots)
            {
                var dr = dt.NewRow();
                dr["ID"] = s.short_id;
                dr["Time"] = s.time;
                dr["Hostname"] = s.hostname;
                dr["Path"] = s.paths[0];
                dt.Rows.Add(dr);
            }
            tv.Table = dt;
        }
    }
}
