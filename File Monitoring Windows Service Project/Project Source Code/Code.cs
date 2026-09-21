using System;
using System.ServiceProcess;
using System.Configuration;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;


namespace File_Monitoring_Windows_Service
{
    public partial class File_Monitoring : ServiceBase
    {
        
        FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();
        string SourceFolder = ConfigurationManager.AppSettings["SourceFolder"];
        string DestinationFolder = ConfigurationManager.AppSettings["DestinationFolder"];
        string LogFolder = ConfigurationManager.AppSettings["LogFolder"];
        string LogFile = Path.Combine(ConfigurationManager.AppSettings["LogFolder"], "ServiceLog.txt");
        public File_Monitoring()
        {
            InitializeComponent();
            CanPauseAndContinue = true;
            CanShutdown = true;
        }

        private void LogServiceEvent(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
            File.AppendAllText(LogFile, logMessage);

            // Writ Events Messages

            //if (Environment.UserInteractive)
            //{
            //    Console.WriteLine(logMessage);
            //}
        }

        void CleanUpResources()
        {
            fileSystemWatcher.EnableRaisingEvents = false;
            fileSystemWatcher.Dispose();
        }

        void CheckFoldersExists()
        {
            if (!Directory.Exists(SourceFolder))
            {
                Directory.CreateDirectory(SourceFolder);
            }

            if (!Directory.Exists(DestinationFolder))
            {
                Directory.CreateDirectory(DestinationFolder);
            }

            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
        }

        bool IsFileCreateComplete(string path)
        {
            try
            {
                using (File.Open(path, FileMode.Open, FileAccess.Read, FileShare.None))
                {
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        bool TestFileReadiness(string path)
        {
            bool IsFileReady = IsFileCreateComplete(path);
            int Timeout = 0;

            if (Directory.Exists(path))
            {
                LogServiceEvent("Erorr: Can't Process Folders");
                 return false;
            }
            

            while (!IsFileReady)
            {
                Thread.Sleep(2000);
                IsFileReady = IsFileCreateComplete(path);
                Timeout++;
                if (Timeout >= 5)
                {
                    LogServiceEvent("Erorr: Unable To Process The File");
                    return false;
                }
            }

            return true;
        }

        bool IsFolder(string path, string name)
        {
            if (Directory.Exists(path))
            {
                LogServiceEvent("Erorr: Can't Process Folder: " + name);
                return true;
            }

            return false;
        }

        void ProcessFile(object sender, FileSystemEventArgs e)
        {
            if (IsFolder(e.FullPath, e.Name))
                return;

            LogServiceEvent("File Detected: " + e.Name);
            LogServiceEvent("Waiting To Process The File: " + e.Name);
            if (!TestFileReadiness(e.FullPath))
                return;
            

            string FilePath = e.FullPath;
            string FileExtension = Path.GetExtension(FilePath);
            string NewFileName = Guid.NewGuid().ToString();

            string NewFullPath = Path.Combine(DestinationFolder,NewFileName + FileExtension);

            try
            {
                File.Move(FilePath, NewFullPath);
                LogServiceEvent("File Moved Successfully");
            }
            catch (IOException ex)
            {
                LogServiceEvent("Erorr: " + ex.Message);
            }


        }

        protected override void OnStart(string[] args)
        {   
            CheckFoldersExists();
            LogServiceEvent("Service Started");
            fileSystemWatcher.Path = SourceFolder;
            fileSystemWatcher.Created += ProcessFile;
            fileSystemWatcher.EnableRaisingEvents = true;
            
        }

        protected override void OnStop()
        {
            CleanUpResources();
            LogServiceEvent("Service Stopped");

        }

        protected override void OnPause()
        {
            LogServiceEvent("Service Pause");
            fileSystemWatcher.EnableRaisingEvents = false;
        }

        protected override void OnContinue()
        {
            LogServiceEvent("Service Continue");
            fileSystemWatcher.EnableRaisingEvents = true;
        }

        protected override void OnShutdown()
        {
            CleanUpResources();
            LogServiceEvent("Service Shutdown");
        }

        //Test Function
        public void TestOnConsole()
        {
            OnStart(null);
            Console.ReadKey();

            Console.WriteLine("Press any Key To Pause The Service ");
            Console.ReadKey();
            OnPause();

            Console.WriteLine("Press any Key To Continue The Service ");
            Console.ReadKey();
            OnContinue();

            Console.WriteLine("Press any Key To Stop The Service ");
            Console.ReadKey();
            OnStop();
        }
    }
}
