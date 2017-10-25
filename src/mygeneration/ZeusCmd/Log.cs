using System;
using System.Collections;
using System.IO;

namespace Zeus
{
    public class Log : ILog
	{
		private string _logFile;
	    private bool _isConsoleEnabled = true;
	    private StreamWriter _writer;

		public Log() {}

		public Log(string fileName)
		{
			_logFile = fileName;
		}

		public void Open()
		{
			if(IsOpen) 
			{
				Close();
			}

			if (_logFile != null) 
			{
				try 
				{
					_writer = File.AppendText(_logFile);
				}
				catch 
				{
					_writer = null;
				}
			}

			IsOpen = (_writer != null);
		}

		public void Close()
		{
			if (_writer != null) 
			{
				_writer.Flush();
				_writer.Close();
				_writer = null;
			}

			IsOpen = false;
		}

		public void Write(string text)
		{

			if(IsLogEnabled) 
			{
				if(!IsOpen) Open();

                if (IsOpen)
                {
                    if (!IsInternalUseMode)
                    {
                        _writer.Write(System.DateTime.Now);
                        _writer.Write(" ");
                    }
                    _writer.Write(text);
                    _writer.WriteLine();
                    _writer.Flush();
                }
			}
			
			if(_isConsoleEnabled)
			{
                if (!IsInternalUseMode)
                {
                    Console.Write(System.DateTime.Now);
                    Console.Write(" ");
                }
				Console.Write(text);
				Console.WriteLine();
				Console.Out.Flush();
			}			
		}
	
		public void Write(string format, params object[] args)
		{
			Write(string.Format(format, args));
		}

		public void Write(Exception ex)
		{
			Write("ERROR: [{0}] - {1}", ex.GetType().FullName, ex.Message);
#if DEBUG
            ArrayList list = new ArrayList();
            bool allGone = false;
            while (!allGone)
            {
                if (!list.Contains(ex)) list.Add(ex);

                if ((ex.InnerException != null) && !list.Contains(ex.InnerException))
                {
                    ex = ex.InnerException;
                }
                else
                {
                    allGone = true;
                }
            }

            foreach (Exception nex in list)
            {
                Write(nex.StackTrace);
            }
#endif
        }

        public bool IsInternalUseMode { get; set; }

	    public bool IsConsoleEnabled
		{
			get { return _isConsoleEnabled; }
			set { _isConsoleEnabled = value; }
		}

		public bool IsLogEnabled { get; set; }

	    public bool IsOpen { get; private set; }

	    public string FileName
		{
			get { return _logFile; }
			set 
			{ 
				if (IsOpen) 
				{
					Close();
				}
				_logFile = value; 	

				if (_logFile != null) 
				{
					Open();
				}
			}
		}
	}
}
