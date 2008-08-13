using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;

using Zeus;
using MyGeneration;

namespace MyGeneration
{
    public static class ZeusProcessManager
    {
        private static Dictionary<Guid, ZeusProcess> processes = new Dictionary<Guid,ZeusProcess>();

        public static Guid ExecuteTemplate(string filename, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteTemplate, callback, filename);
            processes[zp.ID] = zp;
            return zp.ID;
        }

        public static void Kill(Guid pid)
        {
            if (processes.ContainsKey(pid))
            {
                processes[pid].Kill();
                processes.Remove(pid);
            }
        }

        public static void KillAll()
        {
            foreach (Guid pid in processes.Keys)
            {
                processes[pid].Kill();
            }
            processes.Clear();
        }
    }

    public enum ZeusProcessType
    {
        ExecuteTemplate = 0
    }

    public class ZeusProcessStatusEventArgs : EventArgs
    {
        private bool _isRunning = false;
        private string _message = string.Empty;
        private Guid _id = Guid.NewGuid();

        public ZeusProcessStatusEventArgs(Guid id, bool isRunning, string message)
            : base()
        {
            this._id = id;
            this._isRunning = isRunning;
            this._message = message;
        }

        public Guid ID
        {
            get { return _id; }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
        }

        public string Message
        {
            get { return _message; }
        }
    }

    public delegate void ZeusProcessStatusDelegate(ZeusProcessStatusEventArgs success);

    public class ZeusProcess
    {
        private Guid _id = Guid.NewGuid();
        private Thread t;

        private class ThreadData
        {
            public ThreadData(ZeusProcessStatusDelegate c, Process p, Guid id) { Callback = c; SysProcess = p; ID = id; }
            public ZeusProcessStatusDelegate Callback;
            public Process SysProcess;
            public Guid ID;
        }

        public ZeusProcess(ZeusProcessType type, ZeusProcessStatusDelegate callback, params string[] args)
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
            Process process = new Process();
            process.StartInfo = si;
            ParameterizedThreadStart ts = new ParameterizedThreadStart(Start);
            t = new Thread(ts);

            ThreadData td = new ThreadData(callback, process, _id);
            t.Start(td);
        }

        public Guid ID
        {
            get { return _id; }
        }

        public void Kill()
        {
            t.Abort();
        }

        private void Start(object o)
        {
            if (o is ThreadData)
            {
                ThreadData td = o as ThreadData;
                Process p = td.SysProcess;
                try
                {
                    p.Start();
                    while (!p.HasExited)
                    {
                        string l = p.StandardOutput.ReadLine();
                        td.Callback(new ZeusProcessStatusEventArgs(td.ID, true, l));
                        // need to bubble up this line, and the status in a thread safe way.

                        // if marked to "kill", break out and kill process!
                    }
                    td.Callback(new ZeusProcessStatusEventArgs(td.ID, false, "Completed"));
                }
                catch (ThreadAbortException tae)
                {
                    p.Kill();
                    td.Callback(new ZeusProcessStatusEventArgs(td.ID, false, "Killed Process"));
                }
            }
        }
    }
}