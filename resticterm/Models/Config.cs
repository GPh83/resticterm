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
        
        [JsonInclude]
        public String ExtraResticParameters { get; set; } = String.Empty;

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
                if (IsLocalDir(Program.dataManager.config.RepoPath) && !Directory.Exists(Program.dataManager.config.RepoPath))
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

        public bool IsLocalDir(String RepoPath)       
        {
            bool ret=true;

            if (RepoPath.StartsWith("sftp:")) ret = false;
            if (RepoPath.StartsWith("rest:")) ret = false;
            if (RepoPath.StartsWith("s3:")) ret = false;
            if (RepoPath.StartsWith("swift:")) ret = false;
            if (RepoPath.StartsWith("b2:")) ret = false;
            if (RepoPath.StartsWith("azure:")) ret = false;
            if (RepoPath.StartsWith("gs:")) ret = false;
            if (RepoPath.StartsWith("rclone:")) ret = false;

            return ret;
        }
    }
}
