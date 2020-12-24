using System;
using System.Diagnostics;
using System.IO;

namespace Deployer
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string CurFolder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            string srvExec = Path.Combine(CurFolder, "TramSrv.Exe");

            Process.Start(Path.Combine(CurFolder, "InstallUtil.exe"), "\"" + srvExec + "\"");

        }
    }
}
