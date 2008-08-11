using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

using Zeus;
using MyGeneration;

namespace MyGeneration
{
    public delegate void ZeusProcessCompleteDelegate(bool success);

    public static class ZeusProcessManager
    {
        private static Queue<Process> processes;

        public void ExecuteTemplate(string filename, ZeusProcessCompleteDelegate zpcd)
        {
            //
        }
    }
    public enum ZeusProcessType
    {
        ExecuteTemplate = 0
    }
    public class ZeusProcess
    {
        private ParameterizedThreadStart ts;
        private Process process;

        public ZeusProcess(ZeusProcessType type, params string[] args)
        {
            ProcessStartInfo si = new ProcessStartInfo();
            if (type == ZeusProcessType.ExecuteTemplate)
            {
                if (args.Length > 0)
                {
                    si.FileName = FileTools.ApplicationPath + "\\ZeusCmd.exe";
                    si.CreateNoWindow = true;
                    si.UseShellExecute = false;
                    si.RedirectStandardOutput = true;
                    si.Arguments = "-t " + args[0];
                }
            }
            process = new Process();
            process.StartInfo = si;
            ts = new ParameterizedThreadStart(Start);
            ts.Invoke(process);
        }

        private void Start(object o)
        {
            if (o is Process)
            {
                Process p = o as Process;
                p.Start();
                while (!p.HasExited)
                {
                    string l = p.StandardOutput.ReadLine();
                    // need to bubble up this line, and the status in a thread safe way.

                    // if marked to "kill", break out and kill process!
                }

            }
        }
    }
}
