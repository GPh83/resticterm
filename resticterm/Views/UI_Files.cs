﻿using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    /// <summary>
    /// View files details from snapshot
    /// </summary>
    class UI_Files
    {
        TableView tv;
        String currentSnapshotId;

        public void Create(String snapshotId)
        {
            currentSnapshotId = snapshotId;
            var ntop = new Toplevel();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Save", FileSave),
                //new StatusItem(Key.F2, "~F2~ View", FileView),
                new StatusItem(Key.F10, "~F10~ Return", Quit)
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            var win = new Window("Snapshot content")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            ntop.Add(win);

            tv = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true
            };
            win.Add(tv);
            ShowFiles(snapshotId);
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
            FileSave();
        }


        void FileView()
        {
            var fName = tv.Table.Rows[tv.SelectedRow][0];
            var ret = Program.restic._run.Start("dump " + currentSnapshotId + " \"" + fName + "\"", 5000);
            //info.Text += ret;
        }

        void FileSave()
        {
            var filenameToRestore = tv.Table.Rows[tv.SelectedRow][1];
            var saveDialog = new SaveDialog("Restore file(s)", "Choose directory where to restore file(s)");

            saveDialog.DirectoryPath = Path.Combine(Program.dataManager.config.RestorePath,"restore_"+DateTime.Now.ToString("yyyyMMdd"));
            saveDialog.Prompt = "Restore";
            
            Application.Run(saveDialog);
            if (saveDialog.FileName != null)
            {
                var command = "restore " + currentSnapshotId;
                command += " --target \"" + saveDialog.FilePath.ToString() + "\"";
                command += " --include \"" + filenameToRestore + "\"";
                //var command = "dump " + currentSnapshotId;
                //command += " \"" +filenameToRestore+ "\"";
                //command += " --archive \"zip\"";
                ////command += " > " + Path.Combine(saveDialog.FilePath.ToString(), saveDialog.FileName.ToString());

                var ret = Program.restic._run.Start(command, 5000);
                MessageBox.Query("File save", ret, "Ok");
            }
        }

        void ShowFiles(String snapshotId)
        {
            var files = Program.restic.GetFilesFromSnapshots(snapshotId);

            var dt = new DataTable();
            dt.Columns.Add("T");
            dt.Columns.Add("Path");
            dt.Columns.Add("Modif. Time");

            foreach (Models.FileDetails f in files)
            {
                var dr = dt.NewRow();
                switch (f.type)
                {
                    case "file":
                        dr["T"] = "|";
                        break;

                    case "dir":
                        dr["T"] = "+";
                        break;

                    default:
                        dr["T"] = "";
                        break;
                }
                dr["Path"] = f.path;
                dr["Modif. Time"] = f.mtime;
                dt.Rows.Add(dr);
            }
            tv.Table = dt;
        }
    }
}
