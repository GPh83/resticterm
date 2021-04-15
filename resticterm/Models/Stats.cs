using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Models
{
    /// <summary>
    /// Restic JSON Response for stats command
    /// </summary>
    public class Stats
    {
        public int Total_size { get; set; }
        public int Total_file_count { get; set; }
    }
}
