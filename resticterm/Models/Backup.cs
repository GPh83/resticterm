using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Models
{
    /// <summary>
    /// Restic JSON Response for backup command
    /// </summary>
    public class Backup
    {
        /// <summary>
        /// Status during backup
        /// </summary>
        public class Status
        {
            public string message_type { get; set; }
            public float percent_done { get; set; }
            public int total_files { get; set; }
            public int files_done { get; set; }
            public int total_bytes { get; set; }
            public int bytes_done { get; set; }
        }

        /// <summary>
        /// Summary at backup end
        /// </summary>
        public class Summary
        {
            public string message_type { get; set; }
            public int files_new { get; set; }
            public int files_changed { get; set; }
            public int files_unmodified { get; set; }
            public int dirs_new { get; set; }
            public int dirs_changed { get; set; }
            public int dirs_unmodified { get; set; }
            public int data_blobs { get; set; }
            public int tree_blobs { get; set; }
            public int data_added { get; set; }
            public int total_files_processed { get; set; }
            public int total_bytes_processed { get; set; }
            public float total_duration { get; set; }
            public string snapshot_id { get; set; }
        }

    }
}
