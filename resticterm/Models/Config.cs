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


        internal String GetRepoPassword()
        {
            // TODO : Replace fixed password by MasterPassword
            return Libs.Cryptography.Decrypt(EncryptedRepoPassword,"1234");
        }

        internal void SetRepoPassword(String uncryptedPassword)
        {
            // TODO : Replace fixed password by MasterPassword
            EncryptedRepoPassword= Libs.Cryptography.Encrypt(uncryptedPassword, "1234");
        }

    }
}
