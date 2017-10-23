using System;
using System.Diagnostics;

namespace MyGeneration
{
    public class ThreadData
    {
        public ThreadData(ZeusProcessStatusDelegate cbk, Process p, Guid id)
        {
            CallbackHandlers += cbk;
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
}