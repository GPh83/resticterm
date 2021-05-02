using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui;

namespace resticterm.Views
{
    public class UI_Setup
    {
        TextField _repoPath;
        TextField _repoPassword;
        TextField _restorePath;
        TextView _sourcePath;
        TextField _keepLast;
        CheckBox _useMasterPassword;
        CheckBox _useVSS;

        public void ShowModal(String message)
        {
            var ntop = new Toplevel();

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.F1, "~F1~ Save", SaveSetup),
                new StatusItem(Key.Esc, "~Esc~ Return", () => { Quit(); })
            });
            ntop.Add(statusBar);
            ntop.StatusBar = statusBar;

            // Windows
            var win = new Window("Setup")
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };
            ntop.Add(win);

            var ms = new Label(message)
            {
                X = 1,
                Y = 1,
                Width = Dim.Fill(),
                Height = 2,
                ColorScheme = Colors.Error
            };
            ntop.Add(ms);

            Libs.ViewDesign.SetField(ntop, ref _repoPath, "Path to repository", Program.dataManager.config.RepoPath, 30, 5);
            Libs.ViewDesign.SetField(ntop, ref _repoPassword, "Repository password", Program.dataManager.config.GetRepoPassword(), 30, 6);
            Libs.ViewDesign.SetCheck(ntop, ref _useMasterPassword, "Use master password", Program.dataManager.config.UseMasterPassword, 30, 7);
            Libs.ViewDesign.SetCheck(ntop, ref _useVSS, "Windows Volume Shadow Copy (Admin mode needed)", Program.dataManager.config.UseVSS, 30, 8);
            Libs.ViewDesign.SetField(ntop, ref _restorePath, "Restore path", Program.dataManager.config.RestorePath, 30, 10);
            Libs.ViewDesign.SetField(ntop, ref _keepLast, "Purge, keep last", Program.dataManager.config.KeepLastSnapshots.ToString(), 30, 11);
            Libs.ViewDesign.SetField(ntop, ref _sourcePath, "Backup Paths", Program.dataManager.config.SourcesBackupPath, 30, 13, 5);

            //_sourcePath.
            _repoPassword.Secret = true;
            Application.Run(ntop);

        }

        void SaveSetup()
        {
            Program.dataManager.config.RepoPath = _repoPath.Text.ToString();
            Program.dataManager.config.SetRepoPassword(_repoPassword.Text.ToString());
            Program.dataManager.config.UseMasterPassword = _useMasterPassword.Checked;
            Program.dataManager.config.UseVSS = _useVSS.Checked;
            Program.dataManager.config.RestorePath = _restorePath.Text.ToString();
            Program.dataManager.config.KeepLastSnapshots = int.Parse(_keepLast.Text.ToString());
            Program.dataManager.config.SourcesBackupPath = _sourcePath.Text.ToString();
            Program.dataManager.SaveConfig();
            Application.RequestStop();
        }

        void Quit()
        {
            if (!String.IsNullOrWhiteSpace(Program.dataManager.config.EncryptedRepoPassword) && Program.dataManager.config.GetRepoPassword() == "")
            {
                if (MessageBox.ErrorQuery("Bad master pas#word", "Bad master password ! \nIf you continue you must redefine repository password in setup.", "Continue", "Quit") != 0)
                    Environment.Exit(-1);
            }
            Application.RequestStop();

        }

        void Paste()
        {
            //_restorePath.Text = Clipboard.Contents.
            //Debug.WriteLine(Clipboard.Contents.ToString());
        }
    }
}
