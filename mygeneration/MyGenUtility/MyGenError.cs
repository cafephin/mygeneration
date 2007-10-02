using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Zeus;

namespace MyGeneration
{
    public class MyGenError : IMyGenError
    {
        private DateTime dateTimeOccurred;
        private string uniqueIdentifier;
        private MyGenErrorClass errorClass = MyGenErrorClass.Application;
        private bool isWarning = false;
        private bool isRuntime = true;
        private Guid errorGuid = Guid.NewGuid();
        private string errorNumber;
        private string type;
        private string sourceFile;
        private string sourceLine;
        private int lineNumber = 0;
        private int columnNumber = 0;
        private string message;
        private string detail;

        public MyGenError(Exception tex)
        {
            if (tex is Zeus.ErrorHandling.ZeusExecutionException)
            {
                Zeus.ErrorHandling.ZeusExecutionException zex = tex as Zeus.ErrorHandling.ZeusExecutionException;
                Zeus.IZeusExecutionError zee = zex.Errors[0];
            }
            StringBuilder sb = new StringBuilder();

            StackTrace st = new StackTrace(tex.GetBaseException());
            StackFrame[] sfs = st.GetFrames();
            foreach (StackFrame sf in sfs)
            {
                sb.AppendLine(sf.ToString());
            }
            string tmp = sb.ToString();
            tmp = tmp.Trim();
            Exception ex = tex.GetBaseException();
            if (ex == null) ex = tex;

            //StackTrace st = new StackTrace(ex);
            //StackFrame[] sfs = st.GetFrames();
            //foreach (StackFrame sf in sfs)
           // {
                //sb.AppendLine(sf.ToString());
           // }

            dateTimeOccurred = DateTime.Now;
            //errorNumber = 0;
            type = ex.GetType().Name;
            sourceFile = ex.Source;
            //sourceLine = ex.li;
            //lineNumber;
            //columnNumber;
            message = ex.Message;
            detail = ex.StackTrace;
        }

        public MyGenError(IZeusExecutionError ex)
        {
        }

        public DateTime DateTimeOccurred
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string UniqueIdentifier
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public MyGenErrorClass Class
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public bool IsWarning
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public Guid ErrorGuid
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string ErrorNumber
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Type
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string SourceFile
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string SourceLine
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int LineNumber
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public int ColumnNumber
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Message
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        public string Detail
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }
    }
}
