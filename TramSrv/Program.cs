using System.Diagnostics;
using System.ServiceProcess;

namespace TramSrv
{
    static class Program
    {
        public static int ssl_port = 0;
        public static int port = 0;
        public static string sert = "";
        public static string sert_pwd = "";

        public const string loc_port_tag = "-PORT";
        public const string loc_ssl_tag = "-SSLPORT";
        public const string loc_sert_path_tag = "-SERTFILE";
        public const string loc_sert_pwd_tag = "-SERTPWD";

        public static Process main_proc;
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main(string[] args)
        {

            foreach (string arg in args)
            {
                //using (StreamWriter sr = new StreamWriter("c:\\log111.txt", true))
                //{
                //    sr.WriteLine(arg);
                //}

                if (arg.Substring(0, loc_port_tag.Length).ToUpper() == loc_port_tag)
                {
                    try
                    {
                        port = int.Parse(arg.Substring(loc_port_tag.Length, (arg.Length - loc_port_tag.Length)));
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_ssl_tag.Length).ToUpper() == loc_ssl_tag)
                {
                    try
                    {
                        ssl_port = int.Parse(arg.Substring(loc_ssl_tag.Length, (arg.Length - loc_ssl_tag.Length)));
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_sert_path_tag.Length).ToUpper() == loc_sert_path_tag)
                {
                    try
                    {
                        sert = arg.Substring(loc_sert_path_tag.Length, (arg.Length - loc_sert_path_tag.Length)).Trim();
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_sert_pwd_tag.Length).ToUpper() == loc_sert_pwd_tag)
                {
                    try
                    {
                        sert_pwd = arg.Substring(loc_sert_pwd_tag.Length, (arg.Length - loc_sert_pwd_tag.Length)).Trim();
                    }
                    catch { }
                }

            }

            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new TramSrv()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
