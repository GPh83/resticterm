using System;
using System.Collections.Generic;
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

        internal String MasterPassword { get; set; } = String.Empty;

        internal String GetRepoPassword()
        {
            return Libs.Cryptography.Decrypt(EncryptedRepoPassword, MasterPassword);
        }

        internal void SetRepoPassword(String uncryptedPassword)
        {
            EncryptedRepoPassword = Libs.Cryptography.Encrypt(uncryptedPassword, MasterPassword);
        }

    }
}
