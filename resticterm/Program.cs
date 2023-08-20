using System;
using System.IO;
using System.Reflection;
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
            if (HasHelp(args))
            {
                DisplayHelp();
            }
            else
            {
                Application.Init();

                // Retrieve necessary data
                dataManager = new Models.DataManager();
                dataManager.Start();

                // Initialize repo manager
                restic = new Restic.Restic(dataManager.config.RepoPath, dataManager.config.EncryptedRepoPassword);

                // Default Master password 
                dataManager.config.MasterPassword = "8YSZm5bIaWdN6Itd";

                // Start UI
                var main = new Views.UI_Main();
                main.Create();
                Application.Run();
            }
        }

        // Help 
        static bool HasHelp(string[] args)
        {
            bool ret = false;

            ret |= Array.IndexOf(args, "-h") >= 0;
            ret |= Array.IndexOf(args, "--help") >= 0;
            ret |= Array.IndexOf(args, "-v") >= 0;
            ret |= Array.IndexOf(args, "--version") >= 0;
            return ret;
        }

        static void DisplayHelp()
        {
            Console.WriteLine("resticterm version : " + Assembly.GetExecutingAssembly().GetName().Version.ToString());
            Console.WriteLine("");
            Console.WriteLine("resticterm is a multi-platform console UI for restic backup software. (https://restic.net/).");
            Console.WriteLine("It can be used alone as backup tools or with restic command line for manage existing repository.");
            Console.WriteLine("");
            //Console.WriteLine("config file can be in current directory or in home directory");
            Console.WriteLine("config file location : " + Path.Combine(Directory.GetCurrentDirectory(), "recticterm.config.json"));
            Console.WriteLine("");
        }
    }
}
