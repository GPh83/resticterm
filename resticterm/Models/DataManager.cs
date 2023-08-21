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

        public string ConfigFilename { get; set; } = "recticterm.config.json";


        /// <summary>
        /// Load files 
        /// </summary>
        internal void Start()
        {
            LoadConfig();
        }

        internal void LoadConfig()
        {
            if (!File.Exists(ConfigFilename) && File.Exists("config.json")) File.Move("config.json", ConfigFilename);

            if (File.Exists(ConfigFilename))
            {
                String jsonString;
                jsonString = File.ReadAllText(ConfigFilename);
                config = JsonSerializer.Deserialize<Config>(jsonString, new JsonSerializerOptions { WriteIndented = true });
            }
        }

        internal void SaveConfig()
        {
            String jsonString = JsonSerializer.Serialize<Config>(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigFilename, jsonString);
        }

    }
}
