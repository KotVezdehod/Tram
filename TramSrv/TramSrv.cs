using System;
using System.Diagnostics;
using System.IO;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace TramSrv
{
    public partial class TramSrv : ServiceBase
    {
        
        Logger logger = new Logger();
        public TramSrv()
        {
            InitializeComponent();
            this.CanStop = true;
            this.CanPauseAndContinue = false;
            this.AutoLog = true;
        }

        protected override void OnStart(string[] args)
        {

            Thread loggerThread = new Thread(new ThreadStart(logger.Start));
            loggerThread.Start();

            logger.RecordEntry("===== СЛУЖБА: Запускается процесс 'Tram'");

            Task.Factory.StartNew(()=>MainWork());

        }

        protected override void OnStop()
        {
            //logger.Stop();
            Thread.Sleep(1000);


            try
            {
                Program.main_proc.Kill();
                
            }
            catch{}

            
        }

        public void MainWork()
        {

            string args = "";

            if (Program.ssl_port > 0)
            {
                args = Program.loc_ssl_tag + Program.ssl_port.ToString();
                
            }

            if (Program.port > 0)
            {
                args = args + " " + Program.loc_port_tag + Program.port.ToString();
            }

            if (Program.sert.Trim()!="")
            {
                args = args + " " + Program.loc_sert_path_tag + Program.sert;
            }

            if (Program.sert_pwd.Trim() != "")
            {
                args = args + " " + Program.loc_sert_pwd_tag + Program.sert_pwd;
            }

            logger.RecordEntry("СЛУЖБА: Аргументы: " + args);

            var pathToExe = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            Program.main_proc = Process.Start(Path.Combine(pathToExe, "tram.exe"), args);

            Program.main_proc.WaitForExit();

            logger.RecordEntry("===== СЛУЖБА: Процесс 'Tram' завершен");

            logger.RecordEntry("===== СЛУЖБА: Останавливаюсь...");
            ServiceController service = new ServiceController("TramSrv");
            if (!((service.Status.Equals(ServiceControllerStatus.Stopped)) ||
            (service.Status.Equals(ServiceControllerStatus.StopPending))))
            {
                service.Stop();
            }

            logger.RecordEntry("===== СЛУЖБА: Остановился");

            return;
        }

    }

    public class Logger
    {
        FileSystemWatcher watcher;
        object obj = new object();
        bool enabled = true;
        public Logger()
        {
            string currDir = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            watcher = new FileSystemWatcher(currDir);
            watcher.Deleted += Watcher_Deleted;
            watcher.Created += Watcher_Created;
            watcher.Changed += Watcher_Changed;
            watcher.Renamed += Watcher_Renamed;
        }

        public void Start()
        {
            watcher.EnableRaisingEvents = true;
            while (enabled)
            {
                Thread.Sleep(1000);
            }
        }
        public void Stop()
        {
            watcher.EnableRaisingEvents = false;
            enabled = false;
        }
        // переименование файлов
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            string fileEvent = "переименован в " + e.FullPath;
            string filePath = e.OldFullPath;
            RecordEntry(fileEvent, filePath);
        }
        // изменение файлов
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "изменен";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // создание файлов
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "создан";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }
        // удаление файлов
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            string fileEvent = "удален";
            string filePath = e.FullPath;
            RecordEntry(fileEvent, filePath);
        }

        private void RecordEntry(string fileEvent, string filePath)
        {
            if (Program.log_file.Trim() != "")
            {
                lock (obj)
                {
                    using (StreamWriter writer = new StreamWriter(Program.log_file, true))
                    {
                        writer.WriteLine(String.Format("{0} файл {1} был {2}",
                            DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), filePath, fileEvent));
                        writer.Flush();
                    }
                }
            }
        }

        public void RecordEntry(string Event)
        {
            if (Program.log_file.Trim() != "")
            {
                lock (obj)
                {
                    using (StreamWriter writer = new StreamWriter(Program.log_file, true))
                    {
                        writer.WriteLine(String.Format("{0} {1}",
                            DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), Event));
                        writer.Flush();
                    }
                }
            }
        }
    }
}
