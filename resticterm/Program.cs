using System;
using Terminal.Gui;


namespace resticterm
{
    /// <summary>
    /// Main program
    /// </summary>
    internal class Program
    {
        static internal Models.DataManager dataManager;
        static internal Restic.Restic restic;

        static void Main(string[] args)
        {
            Application.Init();

            // Retrieve necessary data
            dataManager = new Models.DataManager();
            dataManager.Start();

            // Initialize repo manager
            restic = new Restic.Restic(dataManager.config.RepoPath, dataManager.config.EncryptedRepoPassword);

            // Start UI
            var main = new Views.UI_Main();
            main.Create();
            Application.Run();

        }
    }
}
