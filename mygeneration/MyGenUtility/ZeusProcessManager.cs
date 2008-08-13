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
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteTemplate, GetCallbacks(callback), filename);
            processes[zp.ID] = zp;
            return zp.ID;
        }

        public static Guid ExecuteProject(string filename, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteProject, GetCallbacks(callback), filename);
            processes[zp.ID] = zp;
            return zp.ID;
        }

        public static Guid ExecuteModule(string filename, string modulePath, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteProjectModule, GetCallbacks(callback), filename, modulePath);
            processes[zp.ID] = zp;
            return zp.ID;
        }

        public static Guid ExecuteProjectItem(string filename, string instancePath, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteProjectItem, GetCallbacks(callback), filename, instancePath);
            processes[zp.ID] = zp;
            return zp.ID;
        }

        public static void Kill(Guid pid)
        {
            lock (processes)
            {
                if (processes.ContainsKey(pid))
                {
                    processes[pid].Kill();
                    processes.Remove(pid);
                }
            }
        }

        public static void KillAll()
        {
            lock (processes)
            {
                foreach (Guid pid in processes.Keys)
                {
                    processes[pid].Kill();
                }
                processes.Clear();
            }
        }

        private static List<ZeusProcessStatusDelegate> GetCallbacks(ZeusProcessStatusDelegate c)
        {
            List<ZeusProcessStatusDelegate> l = new List<ZeusProcessStatusDelegate>();
            l.Add(new ZeusProcessStatusDelegate(LocalCallback));
            if (c != null) l.Add(c);
            return l;
        }

        private static void LocalCallback(ZeusProcessStatusEventArgs args)
        {
            if (!args.IsRunning)
            {
                if (processes.ContainsKey(args.ID))
                {
                    processes[args.ID].Finalize();
                    processes.Remove(args.ID);
                }
            }
        }
    }

    public enum ZeusProcessType
    {
        ExecuteTemplate = 0,
        RecordTemplateInput,
        ExecuteSavedInput,
        ExecuteProject,
        ExecuteProjectModule,
        ExecuteProjectItem,

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
            public ThreadData(List<ZeusProcessStatusDelegate> cbks, Process p, Guid id)
            {
                foreach (ZeusProcessStatusDelegate c in cbks) CallbackHandlers += c;
                SysProcess = p;
                ID = id;
            }
            protected event ZeusProcessStatusDelegate CallbackHandlers;
            public void Callback(ZeusProcessStatusEventArgs args)
            {
                if (CallbackHandlers != null) CallbackHandlers(args);
            }
            public Process SysProcess;
            public Guid ID;
        }

        public ZeusProcess(ZeusProcessType type, List<ZeusProcessStatusDelegate> callbacks, params string[] args)
        {
            ProcessStartInfo si = new ProcessStartInfo();

            if (args.Length > 0)
            {
                si.FileName = FileTools.ApplicationPath + "\\ZeusCmd.exe";
                si.CreateNoWindow = true;
                si.UseShellExecute = false;
                si.RedirectStandardOutput = true;
                string cmdArgs = string.Empty;
                if (type == ZeusProcessType.ExecuteTemplate)
                {
                    cmdArgs = "-t \"" + args[0] + "\"";
                    for (int i = 1; i < args.Length; i++)
                    {
                        if (args[i].Equals("silent", StringComparison.CurrentCultureIgnoreCase))
                            cmdArgs += " -s";
                    }
                }
                else if (type == ZeusProcessType.ExecuteSavedInput)
                {
                    cmdArgs = "-i \"" + args[0] + "\"";
                }
                else if (type == ZeusProcessType.RecordTemplateInput)
                {
                    cmdArgs = "-t \"" + args[0] + "\" -c \"" + args[1] + "\"";
                }
                else if (type == ZeusProcessType.ExecuteProject)
                {
                    cmdArgs = "-p \"" + args[0] + "\"";
                }
                else if (type == ZeusProcessType.ExecuteProjectModule)
                {
                    cmdArgs = "-p \"" + args[0] + "\" -m \"" + args[1] + "\"";
                }
                else if (type == ZeusProcessType.ExecuteProjectItem)
                {
                    cmdArgs = "-p \"" + args[0] + "\" -ti \"" + args[1] + "\"";
                }
                if (!string.IsNullOrEmpty(cmdArgs)) si.Arguments = cmdArgs;
            }
            
            Process process = new Process();
            process.StartInfo = si;
            ParameterizedThreadStart ts = new ParameterizedThreadStart(Start);
            t = new Thread(ts);

            ThreadData td = new ThreadData(callbacks, process, _id);
            t.Start(td);
        }

        public Guid ID
        {
            get { return _id; }
        }

        public bool Finalize()
        {
            bool joinSucceeded = false;
            if (t != null)
            {
                joinSucceeded = t.Join(5 * 1000); // 5 seconds
                t = null;
            }
            return joinSucceeded;
        }

        public bool Kill()
        {
            bool joinSucceeded = false;
            if (t != null)
            {
                t.Abort();
                joinSucceeded = t.Join(5 * 1000); // 5 seconds
                t = null;
            }
            return joinSucceeded;
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
                catch (ThreadAbortException)
                {
                    p.Kill();
                    td.Callback(new ZeusProcessStatusEventArgs(td.ID, false, "Killed Process"));
                }
            }
        }
    }
}