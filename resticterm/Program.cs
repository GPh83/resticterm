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
                // Retrieve necessary data
                dataManager = new Models.DataManager();
                CheckConfigFilename(dataManager, args);
                dataManager.Start();

                Application.Init();

                // Initialize repo manager
                restic = new Restic.Restic(dataManager.config.RepoPath, dataManager.config.EncryptedRepoPassword);

                // Default Master password 
                dataManager.config.MasterPassword = "8YSZm5bIaWdN6Itd";

                // Start UI
                var main = new Views.UI_Main();
                main.Create();
                try
                {
                    Application.Run();
                }
                finally
                {
                    Application.Shutdown();
                }
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
            Console.WriteLine("Using configuration file in current directory : " + Path.Combine(Directory.GetCurrentDirectory(), "recticterm.config.json"));
            Console.WriteLine("unless --config is used.");
            Console.WriteLine("");
            Console.WriteLine("Optionals parameters : ");
            Console.WriteLine("   --config, -c [filepathname] : Use given configuration file (Use \" for long filename with space)");
            Console.WriteLine("   --create : create specified config file by --config if doesn't exist");
            Console.WriteLine("   --help, -h, -v, --version : This help and version");
            Console.WriteLine("");
        }

        static void CheckConfigFilename(Models.DataManager dataManager, string[] args)
        {
            int pos = Array.IndexOf(args, "--config");
            if (pos == -1) pos = Array.IndexOf(args, "-c");
            if (pos >= 0)
            {
                if ((args.Length - 1) > pos)
                {
                    if (File.Exists(args[pos + 1]))
                    {
                        dataManager.ConfigFilename = args[pos + 1];
                    }
                    else
                    {
                        if (Array.IndexOf(args, "--create") >= 0)
                        {
                            dataManager.ConfigFilename = args[pos + 1];
                        }
                        else
                        {
                            Console.WriteLine("Specified config file \"" + args[pos + 1] + "\" not found !");
                            Console.WriteLine("You can use --create for create it.");
                            Environment.Exit(-1);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Config file must be specified after --config option !");
                    Environment.Exit(-1);
                }
            }
        }
    }
}
