using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Timers;


namespace DatabaseBackup_Service
{
    public partial class DatabaseBackup : ServiceBase
    {
        Timer timer;
        TimeSpan Interval;
        string ConnectionString;
        string BackupFolder;
        string LogFolder;
        string LogFile;

        public DatabaseBackup()
        {
            InitializeComponent();
            CanPauseAndContinue = true;
            CanShutdown = true;


            ConnectionString = ConfigurationManager.ConnectionStrings["DataBaseConnection"].ConnectionString;
            BackupFolder = ConfigurationManager.AppSettings["BackupFolder"];
            LogFolder = ConfigurationManager.AppSettings["LogFolder"];
            LogFile = Path.Combine(LogFolder, "ServiceLog.txt");

            int BackupIntervalMinutes;
            if(!int.TryParse(ConfigurationManager.AppSettings["BackupIntervalMinutes"], out BackupIntervalMinutes))
            {
                //if The TryParse Falid
                BackupIntervalMinutes = 60;
            }

            Interval = TimeSpan.FromMinutes(BackupIntervalMinutes);
        }

        private void LogServiceEvent(string message)
        {
            string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}\n";
            File.AppendAllText(LogFile, logMessage);

            //Writ Events Messages

            //if (Environment.UserInteractive)
            //{
            //    Console.WriteLine(logMessage);
            //}
        }

        void CleanUpResources()
        {
            timer.Stop();
            timer.Dispose();
        }

        void CheckFoldersExists()
        {

            if (!Directory.Exists(BackupFolder))
            {
                Directory.CreateDirectory(BackupFolder);               
            }

            if (!Directory.Exists(LogFolder))
            {
                Directory.CreateDirectory(LogFolder);
            }
        }

        void BackupDataBase()
        {
            LogServiceEvent("Backup Started");
            string FileName = $"DVLD-[{DateTime.Now:yyyy-MM-dd_HH-mm-ss}].bak";
            string fullPath = Path.Combine(BackupFolder, FileName);
            try
            {
                string query = $@"BACKUP DATABASE [DVLD] TO DISK = N'{fullPath}' WITH FORMAT";

                using (SqlConnection sqlConnection = new SqlConnection(ConnectionString))
                {
                    sqlConnection.Open();

                    using (SqlCommand command = new SqlCommand(query, sqlConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                LogServiceEvent($"DataBase: [{FileName}] Backup Done Successfully");
            }
            catch (Exception ex)
            {
                LogServiceEvent($"Error: {ex.Message} Backup For DataBase: [{FileName}] Faild");
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            BackupDataBase();
        }

        protected override void OnStart(string[] args)
        {
            CheckFoldersExists();
            LogServiceEvent("Service Started");
            timer = new Timer();
            timer.Interval = Interval.TotalMilliseconds;
            timer.AutoReset = true;
            timer.Elapsed += Timer_Elapsed;
            timer.Start();

        }

        protected override void OnStop()
        {
            CleanUpResources();
            LogServiceEvent("Service Stopped");
        }

        protected override void OnPause()
        {
            timer.Stop();
            LogServiceEvent("Service Pause");
        }

        protected override void OnContinue()
        {
            timer.Start();
            LogServiceEvent("Service Continue");
        }

        protected override void OnShutdown()
        {
            CleanUpResources();
            LogServiceEvent("Service Shutdown");
        }

        // Test Function
        //public void TestOnConsole()
        //{
        //    Console.WriteLine("Press any key to Start");
        //    Console.ReadKey();
        //    OnStart(null);
        //    Console.ReadKey();

        //}
    }
}
