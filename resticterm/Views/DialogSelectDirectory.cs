using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Terminal.Gui;
using Terminal.Gui.Trees;

namespace resticterm.Views
{
    class DialogSelectDirectory
    {
        private Toplevel Top;
        private Window Win;

        /// <summary>
        /// A tree view where nodes are files and folders
        /// </summary>
        TreeView<FileSystemInfo> treeViewFiles;

        private String returPath = "";

        /// <summary>
        /// Directory selector
        /// </summary>
        /// <param name="path">Initial directory</param>
        /// <returns>Selected directory or "" if cancel</returns>
        public String ShowModal(String path)
        {
            returPath = path;
            var myTop = new Toplevel();
            Setup(myTop);
            Application.Run(myTop);
            return returPath;
        }


        private void Setup(Toplevel top)
        {

            Top = top;
            if (Top == null)
            {
                Top = Application.Top;
            }

            Win = new Window("Directory selector")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(1),
            };
            Top.Add(Win);
            Top.LayoutSubviews();

           var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.Esc, "~Esc~ Cancel", () => Quit()),
                new StatusItem(Key.Enter, "~Enter~ Enter", null),
            });
            Top.Add(statusBar);

            var lblFiles = new Label("File Tree:")
            {
                X = 0,
                Y = 1
            };
            Win.Add(lblFiles);

            treeViewFiles = new TreeView<FileSystemInfo>()
            {
                X = 0,
                Y = Pos.Bottom(lblFiles),
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };

            treeViewFiles.ObjectActivated += TreeViewFiles_ObjectActivated;

            SetupFileTree();

            Win.Add(treeViewFiles);
            SetupScrollBar();

            // Current
            FileSystemInfo last = null;
            string[] directories = returPath.Split(Path.DirectorySeparatorChar);
            string full = directories[0];

            if (full.EndsWith(":"))
            {
                full += "\\";        // Windows root
                full = full.ToUpper();
            }

            var di = treeViewFiles.Objects.Where(o => o.FullName == full).FirstOrDefault();
            if (di == null)
            {
                treeViewFiles.AddObject(new DirectoryInfo(full));
            }
            treeViewFiles.ExpandAll(di);

            for (int i = 1; i < directories.Length; i++)
            {
                full = Path.Combine(full, directories[i]);
                di = new DirectoryInfo(full);
                treeViewFiles.Expand(di);
                last = di;
            }
            treeViewFiles.GoTo(last);
            //treeViewFiles.SelectedObject = new DirectoryInfo(full);

         }

        private void SetupScrollBar()
        {
            // When using scroll bar leave the last row of the control free (for over-rendering with scroll bar)
            treeViewFiles.Style.LeaveLastRow = true;

            var _scrollBar = new ScrollBarView(treeViewFiles, true);

            _scrollBar.ChangedPosition += () =>
            {
                treeViewFiles.ScrollOffsetVertical = _scrollBar.Position;
                if (treeViewFiles.ScrollOffsetVertical != _scrollBar.Position)
                {
                    _scrollBar.Position = treeViewFiles.ScrollOffsetVertical;
                }
                treeViewFiles.SetNeedsDisplay();
            };

            _scrollBar.OtherScrollBarView.ChangedPosition += () =>
            {
                treeViewFiles.ScrollOffsetHorizontal = _scrollBar.OtherScrollBarView.Position;
                if (treeViewFiles.ScrollOffsetHorizontal != _scrollBar.OtherScrollBarView.Position)
                {
                    _scrollBar.OtherScrollBarView.Position = treeViewFiles.ScrollOffsetHorizontal;
                }
                treeViewFiles.SetNeedsDisplay();
            };

            treeViewFiles.DrawContent += (e) =>
            {
                _scrollBar.Size = treeViewFiles.ContentHeight;
                _scrollBar.Position = treeViewFiles.ScrollOffsetVertical;
                _scrollBar.OtherScrollBarView.Size = treeViewFiles.GetContentWidth(true);
                _scrollBar.OtherScrollBarView.Position = treeViewFiles.ScrollOffsetHorizontal;
                _scrollBar.Refresh();
            };
        }

        private void SetupFileTree()
        {

            // setup delegates
            treeViewFiles.TreeBuilder = new DelegateTreeBuilder<FileSystemInfo>(

                // Determines how to compute children of any given branch
                GetChildren,
                // As a shortcut to enumerating half the file system, tell tree that all directories are expandable (even if they turn out to be empty later on)				
                (o) => o is DirectoryInfo
            );

            // Determines how to represent objects as strings on the screen
            treeViewFiles.AspectGetter = FileSystemAspectGetter;

            treeViewFiles.AddObjects(DriveInfo.GetDrives().Select(d => d.RootDirectory));
        }

        private void TreeViewFiles_ObjectActivated(ObjectActivatedEventArgs<FileSystemInfo> obj)
        {
            /*            if (obj.ActivatedObject is FileInfo f)
                        {
                            System.Text.StringBuilder sb = new System.Text.StringBuilder();
                            sb.AppendLine($"Path:{f.DirectoryName}");
                            sb.AppendLine($"Size:{f.Length:N0} bytes");
                            sb.AppendLine($"Modified:{ f.LastWriteTime}");
                            sb.AppendLine($"Created:{ f.CreationTime}");

                            MessageBox.Query(f.Name, sb.ToString(), "Close");
                        }
            */
            if (obj.ActivatedObject is DirectoryInfo dir)
            {
                returPath = dir.Name;
                Application.RequestStop();
            }
        }

        //private void ShowLines()
        //{
        //    //miShowLines.Checked = !miShowLines.Checked;

        //    treeViewFiles.Style.ShowBranchLines = true;
        //    treeViewFiles.SetNeedsDisplay();
        //}


        private IEnumerable<FileSystemInfo> GetChildren(FileSystemInfo model)
        {
            // If it is a directory it's children are all contained files and dirs
            if (model is DirectoryInfo d)
            {
                try
                {
                    return d.GetFileSystemInfos()
                        //show directories first
                        .OrderBy(a => a is DirectoryInfo ? 0 : 1)
                        .ThenBy(b => b.Name);
                }
                catch (SystemException)
                {

                    // Access violation or other error getting the file list for directory
                    return Enumerable.Empty<FileSystemInfo>();
                }
            }

            return Enumerable.Empty<FileSystemInfo>(); ;
        }

        private string FileSystemAspectGetter(FileSystemInfo model)
        {
            if (model is DirectoryInfo d)
            {
                return d.Name;
            }
            if (model is FileInfo f)
            {
                return f.Name;
            }

            return model.ToString();
        }

        private void Quit()
        {
            returPath = "";
            Application.RequestStop();
        }
    }
}


