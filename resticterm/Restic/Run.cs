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

    /// <summary>
    /// Running restic binary
    /// </summary>
    public class Run
    {

        internal String _RepoPath { get; }
        internal String _EncryptedPassword { get; }


        #region "Events"
        public event BackupStatusHandler BackupStatus;

        protected void OnBackupStatus(Status status)
        {
            if (BackupStatus != null) BackupStatus(status);
        }
        #endregion

        public Run(String repoPath, String encryptedPassword)
        {
            _RepoPath = repoPath;
            _EncryptedPassword = encryptedPassword;
        }

        /// <summary>
        /// Execute a command and get result
        /// </summary>
        /// <param name="command">Restic command and command parameters</param>
        /// <param name="TimeOut">Time out in ms for waiting command end. -1 for infinite</param>
        /// <param name="stdin">String to send to input</param>
        /// <returns>Command console output </returns>
        public String Start(String command, int TimeOut = -1, String stdin = "")
        {
            var p = new Process();
            var psi = new ProcessStartInfo();
            String ret, err;

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

            // Start info
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            if (!String.IsNullOrEmpty(stdin))
            {
                psi.RedirectStandardInput = true;
            }

            // Password as environment variable
            var pwd = Libs.Cryptography.Decrypt(_EncryptedPassword, Program.dataManager.config.MasterPassword);
            if (String.IsNullOrEmpty(pwd))
            {
                ret = "Invalid master password or password can't be empty !";
            }
            else
            {
                psi.EnvironmentVariables.Add("RESTIC_PASSWORD", Libs.Cryptography.Decrypt(_EncryptedPassword, Program.dataManager.config.MasterPassword));
                //psi.EnvironmentVariables.Add("RESTIC_REPOSITORY", "\"" + _RepoPath + "\"");
                psi.Arguments = command + GetRepo();

                // Execute
                p.StartInfo = psi;
                p.Start();
                if (!String.IsNullOrEmpty(stdin))
                {
                    p.StandardInput.WriteLine(stdin);
                }
                var statusTask = p.StandardOutput.ReadToEndAsync();
                var errorTask = p.StandardError.ReadToEndAsync();
                p.WaitForExit(TimeOut);

                ret = statusTask.Result;
                err = errorTask.Result;
                if (!String.IsNullOrWhiteSpace(err)) ret += "\n" + err;
                p.Close();
            }

            return ret;
        }

        /// <summary>
        /// Do backup
        /// </summary>
        /// <param name="source">File or folder to backup</param>
        /// <param name="flags">optional flags for backup command</param>
        /// <returns>Summary of backup. If error, message_type = "error" and snapshot_id = "message"</returns>
        /// <seealso cref="OnBackupStatus"/>
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
            psi.EnvironmentVariables.Add("RESTIC_PASSWORD", Libs.Cryptography.Decrypt(_EncryptedPassword, Program.dataManager.config.MasterPassword));
            psi.Arguments = "backup " + flags + " \"" + source + "\" " + GetRepo() + " --json ";
            if (Program.dataManager.config.UseVSS && OperatingSystem.IsWindows()) psi.Arguments += " --use-fs-snapshot ";

            p.StartInfo = psi;
            p.Start();

            // Running
            String summary = String.Empty;
            var errorTask = p.StandardError.ReadLineAsync();
            var statusTask = p.StandardOutput.ReadLineAsync();
            while (!p.HasExited)
            {
                // Outpout
                if (statusTask.IsCompleted)
                {
                    var line = statusTask.Result;
                    statusTask = p.StandardOutput.ReadLineAsync();
                    if (!String.IsNullOrWhiteSpace(line))
                    {
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

                // Error
                if (errorTask.IsCompleted)
                {
                    var error = errorTask.Result;
                    errorTask = p.StandardError.ReadLineAsync();
                    if (!String.IsNullOrWhiteSpace(error))
                    {
                        if (error.Contains("error,"))
                        {
                            if (Views.DialogBackupError.AskQuitOnError(error))
                            {
                                p.Kill(true);
                            }
                        }
                    }
                }
            }

            // End
            if (p.ExitCode == 0)
            {
                if (String.IsNullOrWhiteSpace(summary))
                    summary = statusTask.Result;

                //int maxRead = 50;
                //while (String.IsNullOrWhiteSpace(summary) && maxRead > 0)
                //{
                //    summary = p.StandardOutput.ReadLine();
                //    if (String.IsNullOrWhiteSpace(summary) || !summary.Contains("\"message_type\":\"summary\""))
                //        summary = "";
                //    maxRead--;
                //}

                if (!String.IsNullOrWhiteSpace(summary) && summary.Contains("\"message_type\":\"summary\""))
                    ret = JsonSerializer.Deserialize<Summary>(RemoveESC(summary));
                else
                    ret = new Summary { message_type = "error", snapshot_id = "No summary" };
            }
            else
            {
                var error = errorTask.Result;
                ret = new Summary { message_type = "error", snapshot_id = "ExitCode = " + p.ExitCode.ToString() + "\n" + error };
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


        private String GetRepo()
        {
            return " -r \"" + _RepoPath + "\"";
        }

    }

}
