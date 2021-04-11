using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;

namespace resticterm.Models
{
    internal class DataManager
    {
        internal Config config = new();


        /// <summary>
        /// Load files 
        /// </summary>
        internal void Start()
        {
            LoadConfig();
        }

        internal void LoadConfig()
        {
            if (File.Exists("config.json"))
            {
                String jsonString;
                jsonString = File.ReadAllText("config.json");
                config = JsonSerializer.Deserialize<Config>(jsonString);
            }
        }

        internal void SaveConfig()
        {
            String jsonString = JsonSerializer.Serialize<Config>(config);
            File.WriteAllText("config.json", jsonString);
        }

    }
}
