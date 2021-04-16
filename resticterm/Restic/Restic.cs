using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static resticterm.Models.Backup;

namespace resticterm.Restic
{
    public delegate void ProgressHandler(String message, float percent);

    // TODO : Make function for auto-update


    public class Restic
    {
        Run _run;

        #region "Events"
        public event ProgressHandler Progress;

        /// <summary>
        /// Report the current state of backup
        /// </summary>
        /// <param name="message">Action or message</param>
        /// <param name="percent">0 to 1 = Progress bar ans current action. -1 = information message</param>
        protected void OnProgress(String message, float percent)
        {
            if (Progress != null) Progress(message, percent);
        }
        #endregion


        public Restic(String repoPath, String encryptedPassword)
        {
            _run = new Run(repoPath, encryptedPassword);
        }

        /// <summary>
        /// Get repository summary and restic version
        /// </summary>
        /// <returns>Multi lines string</returns>
        public String Summary()
        {
            String ret, rep;

            ret = "\n";

            ret += ">> Repository : " + _run.RepoPath + "\n";
            rep = _run.Start("stats");

            var lines = rep.Split("\n");
            ret += "    " + lines[2] + " \n";
            ret += "    " + lines[3] + " \n";
            ret += "    " + lines[4] + " \n";
            ret += "\n";

            ret += ">> Last backup :\n";
            rep = _run.Start("snapshots latest");
            lines = rep.Split("\n");
            ret += "    " + lines[0] + " \n";
            ret += "    " + lines[2] + " \n";
            ret += "\n";

            rep = _run.Start("version");
            ret += ">> restic version :\n";
            ret += "    " + rep;

            return ret;
        }

        /// <summary>
        /// Do backup and report progress
        /// </summary>
        /// <seealso cref="OnProgress"/>
        public void Backup()
        {
            _run.BackupStatus += BackupStatus;

            // For each source
            var sourcesPath = Program.dataManager.config.SourcesBackupPath.Replace("\r", "").Split("\n");
            foreach (String src in sourcesPath)
            {
                if (!string.IsNullOrWhiteSpace(src))
                {
                    OnProgress("> Source : " + src, -1);
                    OnProgress(src, 0);
                    var summary = _run.StartBackup(src);
                    var filesinfo = "Files : New=" + summary.files_new.ToString();
                    filesinfo += " Changed=" + summary.files_changed.ToString();
                    filesinfo += " Unmodified=" + summary.files_unmodified.ToString();
                    OnProgress(filesinfo, 1);
                    OnProgress("  ID=" + summary.snapshot_id , -1);
                    OnProgress("  " +filesinfo, -1);
                    
                }
            }
            _run.BackupStatus -= BackupStatus;
        }

        private void BackupStatus(Status status)
        {
            OnProgress("Files done : " + status.files_done.ToString(), status.percent_done);
        }
    }
}
