using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Restic
{
    public class Restic
    {
        String _repo;
        String _encryptedPassword;


        public Restic(String repo, String encryptedPassword)
        {
            _repo = " -r " + repo;
            _encryptedPassword = encryptedPassword;
        }


        public String Summary()
        {
            String ret, rep;

            ret = "---- Summary ----\n\n";
            ret += "Repository :\n";
            rep = Run.Start("stats", _repo, _encryptedPassword);

            var lines = rep.Split("\n");
            ret += "    " + lines[2] + " \n";
            ret += "    " + lines[3] + " \n";
            ret += "    " + lines[4] + " \n";
            ret += "\n";

            ret += "Last backup :\n";
            rep = Run.Start("snapshots latest", _repo, _encryptedPassword);
            lines = rep.Split("\n");
            ret += "    " + lines[0] + " \n";
            ret += "    " + lines[2] + " \n";
            ret += "\n";

            rep = Run.Start("version", "", _encryptedPassword);
            ret += "restic version :\n";
            ret += "    " + rep;
            
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


    }
}
