using System;
using Terminal.Gui;


namespace resticterm
{
    /// <summary>
    /// Main program
    /// </summary>
    internal class Program
    {
        #region "Common services"

        static internal Models.DataManager dataManager;
        static internal Restic.Restic restic;

        #endregion


        // Entry point
        static void Main(string[] args)
        {
            Application.Init();
            // TODO : Master password

            // Retrieve necessary data
            dataManager = new Models.DataManager();
            dataManager.Start();

            // Initialize repo manager
            restic = new Restic.Restic(dataManager.config.RepoPath, dataManager.config.EncryptedRepoPassword);

            // TODO : Master password 
            dataManager.config.MasterPassword = "1234";
            
            // Start UI
            var main = new Views.UI_Main();
            main.Create();
            Application.Run();


        }
    }
}
