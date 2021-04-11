using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Restic
{
    public static class Run
    {
         public static String Start(String command, String param, String encryptedPassword)
        {
            var p= new Process();
            var psi = new ProcessStartInfo();

            psi.WorkingDirectory = Path.Combine(Directory.GetCurrentDirectory(),"Restic");
            if (OperatingSystem.IsWindows())
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory ,"restic.exe");
            }
            else
            {
                psi.FileName = Path.Combine(psi.WorkingDirectory, "restic");
            }
            psi.EnvironmentVariables.Add("RESTIC_PASSWORD", Libs.Cryptography.Decrypt(encryptedPassword, "1234"));
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.Arguments = command + " " + param;

            p.StartInfo = psi;
            p.Start();
            p.WaitForExit();

            return p.StandardOutput.ReadToEnd();
        }
    }
}
