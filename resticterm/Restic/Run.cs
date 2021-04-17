using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static resticterm.Models.Backup;

namespace resticterm.Restic
{
    public delegate void BackupStatusHandler(Status status);

    public class Run
    {

        internal String RepoPath { get; }
        internal String EncryptedPassword { get; }


        #region "Events"
        public event BackupStatusHandler BackupStatus;

        protected void OnBackupStatus(Status status)
        {
            if (BackupStatus != null) BackupStatus(status);
        }
        #endregion

        public Run(String repoPath, String encryptedPassword)
        {
            RepoPath = repoPath;
            EncryptedPassword = encryptedPassword;
        }

        /// <summary>
        /// Execute a command and get result
        /// </summary>
        /// <param name="command">Restic command and command parameters</param>
        /// <returns>Command console output </returns>
        public String Start(String command, int TimeOut = -1)
        {
            var p = new Process();
            var psi = new ProcessStartInfo();

            // Select binary
            psi.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Restic");
            if (OperatingSystem.IsWindows())
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic.exe");
            }
            else
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic");
            }
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            // Password
            psi.EnvironmentVariables.Add("RESTIC_PASSWORD", Libs.Cryptography.Decrypt(EncryptedPassword, Program.dataManager.config.MasterPassword));
            psi.Arguments = command + " -r \"" + RepoPath + "\"";

            // Execute
            p.StartInfo = psi;
            p.Start();
            p.WaitForExit(TimeOut);

            var ret = p.StandardOutput.ReadToEnd();
            var err = p.StandardError.ReadToEnd();
            if (!String.IsNullOrWhiteSpace(err)) ret += "\n" + err;

            p.Close();

            return ret;
        }

        /// <summary>
        /// Do backup
        /// </summary>
        /// <param name="source">File or folder to backup</param>
        /// <param name="flags">optional flags for backup command</param>
        /// <returns>Summary of backup. If error, message_type = "error" and snapshot_id = "message"</returns>
        public Summary StartBackup(String source, String flags = "")
        {
            var p = new Process();
            var psi = new ProcessStartInfo();
            Summary ret;

            // Binary
            psi.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Restic");
            if (OperatingSystem.IsWindows())
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic.exe");
            }
            else
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic");
            }
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.EnvironmentVariables.Add("RESTIC_PASSWORD", Libs.Cryptography.Decrypt(EncryptedPassword, Program.dataManager.config.MasterPassword));
            psi.Arguments = "backup " + flags + " \"" + source + "\" -r \"" + RepoPath + "\" --json";

            p.StartInfo = psi;
            p.Start();

            // Running
            String summary = "";
            while (!p.HasExited)
            {
                var line = p.StandardOutput.ReadLine();
                if (line != null)
                {
                    //Debug.WriteLine(ret);
                    if (line.Contains("\"message_type\":\"status\""))
                    {
                        var status = JsonSerializer.Deserialize<Status>(RemoveESC(line));
                        OnBackupStatus(status);
                    }
                    else if (line.Contains("\"message_type\":\"summary\""))
                    {
                        summary = line;
                    }
                }
            }

            // End
            if (p.ExitCode == 0)
            {
                int maxRead = 50;
                while (String.IsNullOrWhiteSpace(summary) && maxRead > 0)
                {
                    summary = p.StandardOutput.ReadLine();
                    if (String.IsNullOrWhiteSpace(summary) || !summary.Contains("\"message_type\":\"summary\""))
                            summary = "";
                    maxRead--;
                }

                if (!String.IsNullOrWhiteSpace(summary) && summary.Contains("\"message_type\":\"summary\""))
                    ret= JsonSerializer.Deserialize<Summary>(RemoveESC(summary));
                else
                    ret= new Summary { message_type = "error", snapshot_id = "No summary" };
            }
            else
            {
                ret= new Summary { message_type = "error", snapshot_id = "ExitCode = " + p.ExitCode.ToString() + "\n" + p.StandardError.ReadToEnd() };
            }
            p.Close();
            p.Dispose();
            return ret;
            
        }

        /// <summary>
        /// Remove VT100 escape code from string
        /// </summary>
        /// <param name="str">Console text</param>
        /// <returns>String without escape code</returns>
        public static String RemoveESC(String str)
        {
            return Regex.Replace(str, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
        }
    }
}
