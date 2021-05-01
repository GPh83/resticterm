using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static resticterm.Models.Backup;

namespace resticterm.Restic
{
    public delegate void ProgressHandler(String message, float percent);

    // TODO : Make function for auto-update

    /// <summary>
    /// Manager for restic binary executable
    /// </summary>
    public class Restic
    {
        public Run _run;        // TODO : Remove public (only for test)

        #region "Events"
        /// <summary>
        /// Report backup progress
        /// </summary>
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

            ret += ">> Repository : " + _run._RepoPath + "\n";
            rep = _run.Start("stats");
            
            var lines = rep.Split("\n");
            if (lines.Length > 3)
            {
                ret += "    " + lines[2] + " \n";
                ret += "    " + lines[3] + " \n";
                ret += "    " + lines[4] + " \n";
                ret += "\n";
            }
            else
            {
                ret = rep + "\n" + "\n";
            }

            ret += ">> Last backup :\n";
            rep = _run.Start("snapshots latest");
            lines = rep.Split("\n");
            if (lines.Length > 1)
            {
                ret += "    " + lines[0] + " \n";
                ret += "    " + lines[2] + " \n";
                ret += "\n";
            }
            else
            {
                ret = rep + "\n" + "\n";
            }

            rep = _run.Start("version");
            ret += ">> restic version :\n";
            ret += "    " + rep + "\n";

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
                    OnProgress("  ID=" + summary.snapshot_id, -1);
                    OnProgress("  " + filesinfo, -1);

                }
            }
            _run.BackupStatus -= BackupStatus;
        }

        public List<Models.SnapshotItem> GetSnapshots(String id = "")
        {
            String rep;
            List<Models.SnapshotItem> snapshots;

            rep = Run.RemoveESC(_run.Start("snapshots " + id + " --json"));
            if (rep.StartsWith('['))
            {
                snapshots = JsonSerializer.Deserialize<List<Models.SnapshotItem>>(rep);
            }
            else
            {
                // If error, return empty
                snapshots = new List<Models.SnapshotItem>();
            }
            return snapshots;
        }

        public List<Models.FileDetails> GetFilesFromSnapshots(String id)
        {
            String rep;

            rep = Run.RemoveESC(_run.Start("ls " + id + " --json"));
            // TODO : Improve this conversion
            rep = RemoveFirstLines(rep, 1).Replace("\n", ",").Replace("\r", "");
            if (rep.EndsWith(",")) rep = rep.Substring(0, rep.Length - 1);
            rep = "[" + rep + "]";
            var files = JsonSerializer.Deserialize<List<Models.FileDetails>>(rep);
            return files;
        }

        public String Check()
        {
            String rep;

            rep = Run.RemoveESC(_run.Start("check"));
            return rep.Replace("\r", "");
        }

        public String Unlock()
        {
            String rep;

            rep = Run.RemoveESC(_run.Start("unlock"));
            return rep.Replace("\r", "");
        }

        public String Create()
        {
            String rep;

            var uncryptedPassword = Program.dataManager.config.GetRepoPassword();
            rep = Run.RemoveESC(_run.Start("init", -1, uncryptedPassword + "\n" + uncryptedPassword));
            return rep.Replace("\r", "");
        }

        public String Purge()
        {
            String rep;
            //forget--keep - last 1--prune
            rep = Run.RemoveESC(_run.Start("forget --keep-last " + Program.dataManager.config.KeepLastSnapshots.ToString() + " --prune"));
            return rep.Replace("\r", "");
        }

        #region "Private"


        private void BackupStatus(Status status)
        {
            String ms = String.Empty;

            // Current files
            if (status.current_files is not null && status.current_files.Length>0 )
                ms += String.Join(" / ", status.current_files[0]) + "\n";
            // File count and time
            ms += "Files: " + status.files_done.ToString() + " / " + status.total_files.ToString();
            ms += "  Time: " + FormatSeconds(status.seconds_elapsed) + " / " + FormatSeconds(status.seconds_remaining);
            OnProgress(ms, status.percent_done);
        }

        public static string RemoveFirstLines(string text, int linesCount)
        {
            var lines = System.Text.RegularExpressions.Regex.Split(text, "\r\n|\r|\n").Skip(linesCount);
            return string.Join(Environment.NewLine, lines.ToArray());
        }

        #endregion

        private String FormatSeconds(long seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            return time.ToString(@"hh\:mm\:ss");
        }

    }
}
