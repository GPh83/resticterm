using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Models
{
    public class FileDetails
    {
        public string name { get; set; }
        public string type { get; set; }
        public string path { get; set; }
        public int uid { get; set; }
        public int gid { get; set; }
        public long mode { get; set; }
        public DateTime mtime { get; set; }
        public DateTime atime { get; set; }
        public DateTime ctime { get; set; }
        public string struct_type { get; set; }
    }
}
