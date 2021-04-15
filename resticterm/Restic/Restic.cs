using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Restic
{
    public delegate void ProgressHandler(String message, int percent);

    public class Restic
    {
        String _repoPath;
        String _encryptedPassword;


        public event ProgressHandler Progress;


        public Restic(String repoPath, String encryptedPassword)
        {
            _repoPath = repoPath;
            _encryptedPassword = encryptedPassword;
        }

        protected void OnProgress(String message, int percent)
        {
            if (Progress != null) Progress(message, percent);
        }

        public String Summary()
        {
            String ret, rep;

            ret = "---- Summary ----\n\n";

            ret += "Repository : " + _repoPath + "\n";
            rep = Run.Start("stats", _repoPath, _encryptedPassword);

            var lines = rep.Split("\n");
            ret += "    " + lines[2] + " \n";
            ret += "    " + lines[3] + " \n";
            ret += "    " + lines[4] + " \n";
            ret += "\n";

            ret += "Last backup :\n";
            rep = Run.Start("snapshots latest", _repoPath, _encryptedPassword);
            lines = rep.Split("\n");
            ret += "    " + lines[0] + " \n";
            ret += "    " + lines[2] + " \n";
            ret += "\n";

            rep = Run.Start("version", "", _encryptedPassword);
            ret += "restic version :\n";
            ret += "    " + rep;


            // TODO : Make function for update
            //rep = Run.Start("self-update", "", _encryptedPassword);
            //if(rep.Contains("up to date"))
            //{
            //    ret += "    restic is up to date\n";
            //}
            //else
            //{
            //    ret += rep;
            //}
            //ret += "\n";

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <seealso cref="OnProgress"/>
        public void Backup()
        {
            OnProgress("Start", 0);

            var sourcesPath = Program.dataManager.config.SourcesBackupPath.Replace("\r","").Split("\n");
            foreach (String src in sourcesPath)
            {              
                if (!string.IsNullOrWhiteSpace(src))
                {
                    OnProgress("  " + src, 10);
                    //var msg = Run.Start("backup \"" + src + "\"", _repoPath, _encryptedPassword);
                    var msg = Run.StartBackup( src , "", _repoPath, _encryptedPassword);
                    OnProgress(msg.ToString(), 100); 

                }
            }

            OnProgress("End", 100);
        }

    }
}
