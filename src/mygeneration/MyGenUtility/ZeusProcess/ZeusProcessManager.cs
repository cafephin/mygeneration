using System;
using System.Collections.Generic;
using System.Threading;

namespace MyGeneration
{
    public static class ZeusProcessManager
    {
        private static Queue<ZeusProcess> processQueue = new Queue<ZeusProcess>();
        private static ZeusProcess runningProcess;
        private static Thread monitorThread;

        public static Guid ExecuteTemplate(string filename, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteTemplate, callback, filename);
            processQueue.Enqueue(zp);
            Start();
            return zp.ID;
        }

        public static Guid ExecuteSavedInput(string filename, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteSavedInput, callback, filename);
            processQueue.Enqueue(zp);
            Start();
            return zp.ID;
        }

        public static Guid RecordTemplateInput(string templateFilename, string saveToFilename, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.RecordTemplateInput, callback, templateFilename, saveToFilename);
            processQueue.Enqueue(zp);
            Start();
            return zp.ID;
        }

        public static Guid ExecuteProject(string filename, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteProject, callback, filename);
            processQueue.Enqueue(zp);
            Start();
            return zp.ID;
        }

        public static Guid ExecuteModule(string filename, string modulePath, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteProjectModule, callback, filename, modulePath);
            processQueue.Enqueue(zp);
            Start();
            return zp.ID;
        }

        public static Guid ExecuteProjectItem(string filename, string instancePath, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.ExecuteProjectItem, callback, filename, instancePath);
            processQueue.Enqueue(zp);
            Start();
            return zp.ID;
        }

        public static Guid RecordProjectItem(string filename, string instancePath, string templateFilename, ZeusProcessStatusDelegate callback)
        {
            ZeusProcess zp = new ZeusProcess(ZeusProcessType.RecordProjectItem, callback, filename, instancePath, templateFilename);
            processQueue.Enqueue(zp);
            Start();
            return zp.ID;
        }

        public static void Start()
        {
            if ((monitorThread == null) ||
                (monitorThread.ThreadState == System.Threading.ThreadState.Aborted) ||
                (monitorThread.ThreadState == System.Threading.ThreadState.Stopped))
            {
                monitorThread = new Thread(new ThreadStart(runMonitorThread));
            }

            if  (monitorThread.ThreadState == System.Threading.ThreadState.Unstarted)
            {
                monitorThread.Start();
            }
        }

        private static void runMonitorThread()
        {
            try
            {
                while (processQueue.Count > 0 || runningProcess != null)
                {
                    if (runningProcess == null)
                    {
                        runningProcess = processQueue.Dequeue();
                    }

                    runningProcess.Start();
                    while (!runningProcess.IsDormant)
                    {
                        Thread.Sleep(250);
                    }
                    runningProcess.Join();
                    runningProcess = null;
                }
            }
            catch (ThreadAbortException)
            {
                if (runningProcess != null)
                {
                    runningProcess.Kill();
                }
                processQueue.Clear();
            }
        }

        public static int ProcessCount
        {
            get
            {
                if (monitorThread != null) return processQueue.Count;
                else return 0;
            }
        }

        public static bool IsDormant
        {
            get
            {
                if (monitorThread == null) return true;
                else
                {
                    if (monitorThread.ThreadState == System.Threading.ThreadState.Aborted ||
                        monitorThread.ThreadState == System.Threading.ThreadState.Stopped ||
                        monitorThread.ThreadState == System.Threading.ThreadState.Unstarted)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public static void KillAll()
        {
            if (monitorThread != null)
            {
                monitorThread.Abort();
            }

            monitorThread.Join(10 * 1000);
            monitorThread = null;
        }
    }

    public delegate void ZeusProcessStatusDelegate(ZeusProcessStatusEventArgs success);
}