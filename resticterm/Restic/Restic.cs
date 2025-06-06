﻿using resticterm.Libs;
using resticterm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static resticterm.Models.Backup;

namespace resticterm.Restic
{
    public delegate void ProgressHandler(String message, float percent);

    /// <summary>
    /// Manager for restic binary executable
    /// </summary>
    public class Restic
    {
        Run _run;
        string _SummaryCache = string.Empty;
        bool _CacheRefreshNeeded = true;

        #region "Events"
        /// <summary>
        /// Report backup progress
        /// </summary>
        public event ProgressHandler Progress;

        /// <summary>
        /// Report the current state of backup
        /// </summary>
        /// <param name="message">Action or message</param>
        /// <param name="percent">0 to 1 = Progress bar and current action. -1 = information message</param>
        protected void OnProgress(String message, float percent)
        {
            if (Progress != null) Progress(message, percent);
        }
        #endregion


        public Restic(String repoPath, String encryptedPassword)
        {
            _run = new Run(repoPath, encryptedPassword);
        }


        public void ForceRefreshSummaryCache()
        {
            _CacheRefreshNeeded = true;
        }


        /// <summary>
        /// Get repository summary and restic version
        /// </summary>
        /// <returns>Multi lines string</returns>
        /// <remarks>Internaly use cache for speed up</remarks>
        public String Summary()
        {
            String ret, rep;

            if (_CacheRefreshNeeded)
            {
                ret = "\n";

                ret += ">> Config file : " + Program.dataManager.ConfigFilename + "\n";

                ret += ">> Repository : " + _run._RepoPath + "\n";

                ret += "    Version : " + GetVersion().ToString() + "\n";

                rep = _run.Start("stats");

                var lines = rep.Split("\n");
                if (lines.Length > 3)
                {
                    ret += "    " + lines[2] + " \n";
                    ret += "    " + lines[3] + " \n";
                    if (lines.Length > 4) ret += "    " + lines[4] + " \n";
                }
                else
                {
                    ret = rep + "\n";
                }
                if (Program.dataManager.config.IsLocalDir(_run._RepoPath))
                {
                    ret += "Repository disk space : " + DirectoryTools.BytesToHumanString(DirectoryTools.DirSize(_run._RepoPath)) + "\n";
                }
                ret += "\n";

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

                _SummaryCache = ret;
                _CacheRefreshNeeded = false;
            }
            else
            {
                ret = _SummaryCache;
            }

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
            OnProgress("End", 100);
            OnProgress("End", -1);
            _CacheRefreshNeeded = true;
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

        public void Restore(String snapshotID, String filepath, String filenameToRestore)
        {
            _run.RestoreStatus += RestoreStatus;

            OnProgress("\nStart", -1);
            OnProgress("Started", 0);


            var summary = _run.StartRestore(snapshotID, filepath, filenameToRestore);
            OnProgress("Done", 100);
            OnProgress("End\n", -1);
            if (summary.message_type == "error")
            {
                OnProgress("Error !" , -1);
            }
            else
            {
                OnProgress("Files restored : " + summary.files_restored.ToString(), -1);
            }
            _run.RestoreStatus -= RestoreStatus;
        }

        public String Remove(String snapshotID)
        {
            var command = "forget";
            command += " " + snapshotID;
            command += " -v";
            _CacheRefreshNeeded = true;
            return Program.restic._run.Start(command);
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
            _CacheRefreshNeeded = true;
            return rep.Replace("\r", "");
        }

        public String Purge()
        {
            String rep;
            //forget--keep - last 1--prune
            rep = Run.RemoveESC(_run.Start("forget --keep-last " + Program.dataManager.config.KeepLastSnapshots.ToString() + " --prune"));
            _CacheRefreshNeeded = true;
            return rep.Replace("\r", "");
        }

        public String Prune()
        {
            String rep;
            //forget--keep - last 1--prune
            rep = "\n" + Run.RemoveESC(_run.Start("prune"));
            _CacheRefreshNeeded = true;
            return rep.Replace("\r", "");
        }

        public String UpgradeV2()
        {
            String rep;
            var uncryptedPassword = Program.dataManager.config.GetRepoPassword();

            rep = "\n" + Run.RemoveESC(_run.Start("migrate upgrade_repo_v2", -1, uncryptedPassword + "\n" + uncryptedPassword));
            _CacheRefreshNeeded = true;
            return rep.Replace("\r", "");
        }


        public int GetVersion()
        {
            String rep;
            CatConfig ver;

            try
            {
                rep = Run.RemoveESC(_run.Start("cat config"));
                ver = JsonSerializer.Deserialize<Models.CatConfig>(rep);

            }
            catch (Exception)
            {
                return -1;
            }

            return ver.version;
        }

        #region "Private"


        private void BackupStatus(Status status)
        {
            String ms = String.Empty;

            // Current files
            if (status.current_files is not null && status.current_files.Length > 0)
                ms += String.Join(" / ", status.current_files[0]) + "\n";
            // File count and time
            ms += "Files: " + status.files_done.ToString() + " / " + status.total_files.ToString();
            if (status.seconds_elapsed < long.MaxValue && status.seconds_remaining < long.MaxValue)
                ms += "  Time: " + FormatSeconds((long)status.seconds_elapsed) + " / " + FormatSeconds((long)status.seconds_remaining);
            OnProgress(ms, status.percent_done);
        }


        private void RestoreStatus(Models.Restore status)
        {
            String ms = String.Empty;

            // File count 
            ms += "Files: " + status.files_restored.ToString() + " / " + status.total_files.ToString();
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
