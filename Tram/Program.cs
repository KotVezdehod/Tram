using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Hosting.WindowsServices;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Tram
{
    public static class Args
    {
        public static int ssl_port = 0;
        public static int port = 0;
        public static string sert = "";
        public static string sert_pwd = "";

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            string loc_port_tag = "-PORT";
            string loc_ssl_tag = "-SSLPORT";
            string loc_sert_path_tag = "-SERTFILE";
            string loc_sert_pwd_tag = "-SERTPWD";

            foreach (string arg in args)
            {
                if (arg.Substring(0, loc_port_tag.Length).ToUpper() == loc_port_tag)
                {
                    try
                    {
                        Args.port = int.Parse(arg.Substring(loc_port_tag.Length, (arg.Length - loc_port_tag.Length)));
                    }
                    catch{}
                }
                
                if (arg.Substring(0, loc_ssl_tag.Length).ToUpper() == loc_ssl_tag)
                {
                    try
                    {
                        Args.ssl_port = int.Parse(arg.Substring(loc_ssl_tag.Length, (arg.Length - loc_ssl_tag.Length)));
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_sert_path_tag.Length).ToUpper() == loc_sert_path_tag)
                {
                    try
                    {
                        Args.sert = arg.Substring(loc_sert_path_tag.Length, (arg.Length - loc_sert_path_tag.Length)).Trim();
                    }
                    catch { }
                }

                if (arg.Substring(0, loc_sert_pwd_tag.Length).ToUpper() == loc_sert_pwd_tag)
                {
                    try
                    {
                        Args.sert_pwd = arg.Substring(loc_sert_pwd_tag.Length, (arg.Length - loc_sert_pwd_tag.Length)).Trim();
                    }
                    catch { }
                }

            }

            //// получаем путь к файлу 
            //var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            //// путь к каталогу проекта
            //var pathToContentRoot = Path.GetDirectoryName(pathToExe);
            //// создаем хост

            //var host = WebHost.CreateDefaultBuilder(args)
            //    .UseContentRoot(pathToContentRoot)
            //    .UseStartup<Startup>()
            //    .Build();
            //host.RunAsService();

            CreateHostBuilder(args).Build().Run();

        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    //webBuilder.UseUrls("http://0.0.0.0:5010");
                });

    }
}
