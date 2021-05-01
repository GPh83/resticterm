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
            public long seconds_elapsed { get; set; }
            public long seconds_remaining { get; set; }
            public float percent_done { get; set; }
            public long total_files { get; set; }
            public long files_done { get; set; }
            public long total_bytes { get; set; }
            public long bytes_done { get; set; }
            public string[] current_files { get; set; }
        }


        /// <summary>
        /// Summary at backup end
        /// </summary>
        public class Summary
        {
            public string message_type { get; set; }
            public long files_new { get; set; }
            public long files_changed { get; set; }
            public long files_unmodified { get; set; }
            public long dirs_new { get; set; }
            public long dirs_changed { get; set; }
            public long dirs_unmodified { get; set; }
            public long data_blobs { get; set; }
            public long tree_blobs { get; set; }
            public long data_added { get; set; }
            public long total_files_processed { get; set; }
            public long total_bytes_processed { get; set; }
            public float total_duration { get; set; }
            public string snapshot_id { get; set; }
        }
    }
}
