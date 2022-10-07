using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace resticterm.Models
{
    /// <summary>
    /// Restic JSON response for cat config command
    /// </summary>
    internal class CatConfig
    {
        public int version { get; set; }
        public string id { get; set; }
        public string chunker_polynomial { get; set; }
    }
}
