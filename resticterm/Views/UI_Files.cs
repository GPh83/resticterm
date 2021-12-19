using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;
using static Terminal.Gui.TableView;

namespace resticterm.Views
{
    /// <summary>
    /// View files details from snapshot
    /// </summary>
    class UI_Files
    {
        Toplevel ntop;
        Window win;
        TableView tv;
        String currentSnapshotId;

        public void ShowModal(String snapshotId)
        {
            currentSnapshotId = snapshotId;
            ntop = new Toplevel();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.Enter, "~Enter~ Restore", FileRestore),
                new StatusItem(Key.F3, "~F3~ Filter", Filter),
                new StatusItem(Key.Esc, "~Esc~ Return", Quit)
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            win = new Window("Snapshot content")
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
            FileRestore();
        }

        void FileRestore()
        {
            //String SaveTo = Path.Combine(Program.dataManager.config.RestorePath, "restore_" + DateTime.Now.ToString("yyyyMMdd"));
            //if (!Directory.Exists(SaveTo)) Directory.CreateDirectory(SaveTo);

            var filenameToRestore = tv.Table.Rows[tv.SelectedRow][1].ToString();
            var openDialog = new OpenDialog("Restore file(s)", "Choose directory where to restore file(s)", null, OpenDialog.OpenMode.Directory);
            openDialog.NameDirLabel = "Directory";
            openDialog.NameFieldLabel = "Restore to folder";
            openDialog.IsExtensionHidden = true;
            openDialog.AllowedFileTypes = null;
            openDialog.CanCreateDirectories = true;
            openDialog.DirectoryPath = Path.GetDirectoryName(Program.dataManager.config.RestorePath.TrimEnd(Path.DirectorySeparatorChar));
            openDialog.FilePath = Path.GetFileName(Program.dataManager.config.RestorePath.TrimEnd(Path.DirectorySeparatorChar));
            openDialog.Prompt = "Restore";

            Application.Run(openDialog);
            if (!openDialog.Canceled)
            {
                var ret = Program.restic.Restore(currentSnapshotId, openDialog.FilePaths[0].ToString(), filenameToRestore);
                MessageBox.Query("File(s) save", ret, "Ok");
            }
        }

        void ShowFiles(String snapshotId, String Filter = "")
        {
            var files = Program.restic.GetFilesFromSnapshots(snapshotId);

            var dt = new DataTable();
            dt.Columns.Add("T");
            dt.Columns.Add("Path");
            dt.Columns.Add("Modif. Time");

            foreach (Models.FileDetails f in files)
            {
                if (String.IsNullOrWhiteSpace(Filter) || f.path.Contains(Filter))
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
            }
            tv.Table = dt;
        }


        void Filter()
        {
            var filter = InputBox.ShowModal("Filter", "Word to search :", "", "Show all path or file containing the text.\nCase sensitive.\nEmpty for all.");
            if (filter != null)
            {
                ShowFiles(currentSnapshotId, filter);
                if (filter == "")
                    win.Title = "Snapshot content";
                else
                    win.Title = "Snapshot content  Filter=[" + filter + "]";
                //Libs.ViewDesign.RefreshView(ntop, win);
            }

        }
    }
}

