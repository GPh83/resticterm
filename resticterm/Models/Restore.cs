using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Models
{
    public class Restore
    {
        // Common 
        public string message_type { get; set; }
        public ulong total_files { get; set; }
        public ulong total_bytes { get; set; }
        
        // Only "message_type":"status"
        public float percent_done { get; set; }

        // Only "message_type":"summary"
        public ulong files_restored { get; set; }
        public ulong bytes_restored { get; set; }
    }

}
