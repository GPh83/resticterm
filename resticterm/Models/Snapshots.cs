using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Models
{
    /// <summary>
    /// Restic JSON Response for snapshots command
    /// </summary>
    public class SnapshotItem
    {
        public DateTime time { get; set; }
        public string tree { get; set; }
        public string[] paths { get; set; }
        public string hostname { get; set; }
        public string username { get; set; }
        public string id { get; set; }
        public string short_id { get; set; }
        public string parent { get; set; }
        public string struct_type { get; set; }
    }

}

