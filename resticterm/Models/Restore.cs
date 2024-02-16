using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Models
{
    class Restore
    {
        // Common 
        public string message_type { get; set; }
        public int total_files { get; set; }
        public int total_bytes { get; set; }
        
        // Only "message_type":"status"
        public int percent_done { get; set; }

        // Only "message_type":"summary"
        public int files_restored { get; set; }
        public int bytes_restored { get; set; }
    }

}
