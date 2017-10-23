using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Zeus;

namespace MyGeneration
{
    public class ZeusProcess
    {
        private Guid _id = Guid.NewGuid();
        private Thread t;
        private ThreadData td;

        public ZeusProcess(ZeusProcessType type, ZeusProcessStatusDelegate callback, params string[] args)
        {
            ProcessStartInfo si = new ProcessStartInfo();

            if (args.Length > 0)
            {
                si.FileName = FileTools.ApplicationPath + "\\ZeusCmd.exe";
                si.CreateNoWindow = true;
                si.UseShellExecute = false;
                si.RedirectStandardOutput = true;
                string cmdArgs = "-internaluse ";
                if (type == ZeusProcessType.ExecuteTemplate)
                {
                    cmdArgs += "-t \"" + args[0] + "\"";
                }
                else if (type == ZeusProcessType.ExecuteSavedInput)
                {
                    cmdArgs += "-i \"" + args[0] + "\"";
                }
                else if (type == ZeusProcessType.RecordTemplateInput)
                {
                    cmdArgs += "-t \"" + args[0] + "\" -c \"" + args[1] + "\"";
                }
                else if (type == ZeusProcessType.ExecuteProject)
                {
                    cmdArgs += "-p \"" + args[0] + "\"";
                }
                else if (type == ZeusProcessType.ExecuteProjectModule)
                {
                    cmdArgs += "-p \"" + args[0] + "\" -m \"" + args[1] + "\"";
                }
                else if (type == ZeusProcessType.ExecuteProjectItem)
                {
                    cmdArgs += "-p \"" + args[0] + "\" -ti \"" + args[1] + "\"";
                }
                else if (type == ZeusProcessType.RecordProjectItem)
                {
                    //filename, instancePath, templateFilename
                    //-internaluse  -t "C:\projects\mygeneration\trunk\templates\HTML\HTML_DatabaseReport.csgen" -p "c:\PrjRoot.zprj" -rti "/PrjRoot/testInstance"
                    cmdArgs += "-t \"" + args[2] + "\" -p \"" + args[0] + "\" -rti \"" + args[1] + "\"";
                }
                if (!string.IsNullOrEmpty(cmdArgs)) si.Arguments = cmdArgs;
            }
            
            Process process = new Process();
            process.StartInfo = si;
            ParameterizedThreadStart ts = new ParameterizedThreadStart(Start);
            t = new Thread(ts);
            td = new ThreadData(callback, process, _id);

        }

        public void Start()
        {
            t.Start(td);
        }

        public Guid ID
        {
            get { return _id; }
        }

        public bool IsDormant
        {
            get 
            {
                if (t == null) return true;
                else
                {
                    if (t.ThreadState == System.Threading.ThreadState.Aborted ||
                        t.ThreadState == System.Threading.ThreadState.Stopped ||
                        t.ThreadState == System.Threading.ThreadState.Unstarted)
                    {
                        return true;
                    }
                }
                return false;
            }
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

        public bool Join()
        {
            bool joinSucceeded = false;
            if (t != null)
            {
                joinSucceeded = t.Join(5 * 1000); // 5 seconds max
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
                    string l;
                    p.Start();
                    StringBuilder error;
                    do {
                        l = p.StandardOutput.ReadLine();
                        if (l != null) {
                            if (l.StartsWith(ZeusProcessManagerTags.ERROR_TAG)) {
                                error = new StringBuilder(l);
                                do {
                                    l = p.StandardOutput.ReadLine();
                                    if (l.StartsWith("   "))
                                        error.AppendLine(l);
                                    else {
                                        td.Callback(new ZeusProcessStatusEventArgs(td.ID, true, error.ToString()));
                                        break;
                                    }
                                } while (l != null);
                                error = null;
                            }

                            td.Callback(new ZeusProcessStatusEventArgs(td.ID, true, l));
                        }
                        // need to bubble up this line, and the status in a thread safe way.

                        // if marked to "kill", break out and kill process!
                    }
                    while (l != null);

                    td.Callback(new ZeusProcessStatusEventArgs(td.ID, false, "Completed"));
                }
                catch (ThreadAbortException)
                {
                    try
                    {
                        if (p != null && !p.HasExited) p.Kill();
                    }
                    catch { }
                    //td.Callback(new ZeusProcessStatusEventArgs(td.ID, false, "Killed Process"));
                }
            }
        }
    }
}