using System;

namespace Zeus
{
	public interface ILog
	{
		void Write(string text);
		void Write(string format, params string[] args);
		void Write(Exception ex);
	}
}
