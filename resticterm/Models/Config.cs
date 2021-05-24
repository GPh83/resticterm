using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace resticterm.Models
{
    public class Config
    {
        [JsonInclude]
        public String RepoPath { get; set; } = String.Empty;

        [JsonInclude]
        public String EncryptedRepoPassword { get; set; } = String.Empty;

        [JsonInclude]
        public String SourcesBackupPath { get; set; } = String.Empty;

        [JsonInclude]
        public String RestorePath { get; set; } = String.Empty;

        [JsonInclude]
        public bool UseMasterPassword { get; set; } = false;

        [JsonInclude]
        public int KeepLastSnapshots { get; set; } = 60;

        [JsonInclude]
        public bool UseVSS { get; set; } = false;


        // --use-fs-snapshot
        internal String MasterPassword { get; set; } = String.Empty;

        internal String GetRepoPassword()
        {
            return Libs.Cryptography.Decrypt(EncryptedRepoPassword, MasterPassword);
        }

        internal void SetRepoPassword(String uncryptedPassword)
        {
            EncryptedRepoPassword = Libs.Cryptography.Encrypt(uncryptedPassword, MasterPassword);
        }


        internal String CheckValidity()
        {
            String ret = "";

            if (String.IsNullOrWhiteSpace(Program.dataManager.config.RepoPath))
                ret += "Repository path not defined !\n";

            if (String.IsNullOrWhiteSpace(Program.dataManager.config.EncryptedRepoPassword))
            {
                ret += "Repository password not defined !\n";
            }
            else
            {
                if (Program.dataManager.config.GetRepoPassword() == "")
                    ret += "Invalid master password !\n";
            }

            if (ret == "")
            {
                if (!Program.dataManager.config.RepoPath.StartsWith("sftp:") && !Directory.Exists(Program.dataManager.config.RepoPath))
                {
                    try
                    {
                        Directory.CreateDirectory(Program.dataManager.config.RepoPath);
                    }
                    catch (Exception)
                    {
                        ret += "Can't create Repository !\n";
                    }
                }
            }
            return ret;
        }


    }
}
