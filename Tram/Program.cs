using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Tram
{
    public static class Args
    {
        public static int ssl_port = 0;
        public static int port = 0;
        public static string sert = "";
        public static string sert_pwd = "";
        public static string log_file = "";
    }

    public static class logger
    {
        public static object obj = new object();

        public static void RecordEntry(string Event)
        {
            if (Args.log_file.Trim() != "")
            {
                lock (obj)
                {
                    using (StreamWriter writer = new StreamWriter(Args.log_file, true))
                    {
                        writer.WriteLine(String.Format("{0} {1}",
                            DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss"), Event));
                        writer.Flush();
                    }
                }
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string loc_port_tag = "-PORT";
            string loc_ssl_tag = "-SSLPORT";
            string loc_sert_path_tag = "-SERTFILE";
            string loc_sert_pwd_tag = "-SERTPWD";
            string loc_log_file = "-LOGFILE";

            logger.RecordEntry("=== Tram: я запустился.");

            foreach (string arg in args)
            {
                if (arg.Substring(0, loc_log_file.Length).ToUpper() == loc_log_file)
                {
                    try
                    {
                        Args.log_file = arg.Substring(loc_log_file.Length, (arg.Length - loc_log_file.Length));

                        logger.RecordEntry("Tram: найден аргумент: '" + loc_log_file + "'");
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_port_tag.Length).ToUpper() == loc_port_tag)
                {
                    try
                    {
                        Args.port = int.Parse(arg.Substring(loc_port_tag.Length, (arg.Length - loc_port_tag.Length)));
                        logger.RecordEntry("Tram: найден аргумент: '" + loc_port_tag + "'");
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_ssl_tag.Length).ToUpper() == loc_ssl_tag)
                {
                    try
                    {
                        Args.ssl_port = int.Parse(arg.Substring(loc_ssl_tag.Length, (arg.Length - loc_ssl_tag.Length)));
                        logger.RecordEntry("Tram: найден аргумент: '" + loc_ssl_tag + "'");
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_sert_path_tag.Length).ToUpper() == loc_sert_path_tag)
                {
                    try
                    {
                        Args.sert = arg.Substring(loc_sert_path_tag.Length, (arg.Length - loc_sert_path_tag.Length)).Trim();
                        logger.RecordEntry("Tram: найден аргумент: '" + loc_sert_path_tag + "'");
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_sert_pwd_tag.Length).ToUpper() == loc_sert_pwd_tag)
                {
                    try
                    {
                        Args.sert_pwd = arg.Substring(loc_sert_pwd_tag.Length, (arg.Length - loc_sert_pwd_tag.Length)).Trim();
                        logger.RecordEntry("Tram: найден аргумент: '" + loc_sert_pwd_tag + "'");
                    }
                    catch { }
                }

            }

            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

    }
}
