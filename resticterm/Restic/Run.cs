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

    public static class Run
    {
        //public event BackupStatusHandler Status;

        public static String Start(String command, String repoPath, String encryptedPassword)
        {
            var p = new Process();
            var psi = new ProcessStartInfo();

            psi.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Restic");
            if (OperatingSystem.IsWindows())
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic.exe");
            }
            else
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic");
            }
            psi.EnvironmentVariables.Add("RESTIC_PASSWORD", Libs.Cryptography.Decrypt(encryptedPassword, Program.dataManager.config.MasterPassword));
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.Arguments = command + " -r \"" + repoPath + "\"";

            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();

            return p.StandardOutput.ReadToEnd() + "\n" + p.StandardError.ReadToEnd();
        }


        public static Summary StartBackup(String source, String flags, String repoPath, String encryptedPassword)
        {
            var p = new Process();
            var psi = new ProcessStartInfo();
            String summary;

            psi.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Restic");
            if (OperatingSystem.IsWindows())
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic.exe");
            }
            else
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic");
            }
            psi.EnvironmentVariables.Add("RESTIC_PASSWORD", Libs.Cryptography.Decrypt(encryptedPassword, Program.dataManager.config.MasterPassword));
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.Arguments = "backup " + flags + " \"" + source + "\" -r \"" + repoPath + "\" --json";

            p.StartInfo = psi;
            p.Start();

            summary = "";
            while (!p.HasExited)
            {
                var ret = p.StandardOutput.ReadLine();
                if (ret != null)
                {
                    if (ret.Contains("\"message_type\":\"status\""))
                    {
                        var status = JsonSerializer.Deserialize<Status>(RemoveESC(ret));
                        Debug.WriteLine((status.percent_done * 100).ToString());
                    }

                    if (ret.Contains("\"message_type\":\"summary\""))
                    {
                        summary = ret;
                    }
                    ret="";
                }
            }

            if (p.ExitCode == 0)
            {
                if (String.IsNullOrWhiteSpace(summary))
                    summary = p.StandardOutput.ReadLine();

                if (summary != null && summary.Contains("\"message_type\":\"summary\""))
                    return JsonSerializer.Deserialize<Summary>(RemoveESC(summary));
                else
                    return new Summary { message_type = "error", snapshot_id = "No summary" };
            }
            else
            {
                return new Summary { message_type = "error", snapshot_id = "ExitCode = " + p.ExitCode.ToString() };
            }

        }

        public static String RemoveESC(String str)
        {
            return Regex.Replace(str, @"\e\[(\d+;)*(\d+)?[ABCDHJKfmsu]", "");
        }
    }
}
