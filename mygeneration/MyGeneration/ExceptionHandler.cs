using System;
using System.Windows.Forms;
using System.Threading;

namespace MyGeneration
{
	/// <summary>
	/// Summary description for GlobalExceptionHandler.
	/// </summary>
	public class ExceptionHandler
	{
		static public void UnhandledExceptions(object sender, UnhandledExceptionEventArgs args)
		{
			try
			{
				// Most likey the application is terminating on this method
				Exception ex = (Exception)args.ExceptionObject;
				HandleError(ex);
			}
			catch {}
		}

		static public void OnThreadException(object sender, ThreadExceptionEventArgs t)
		{
			try
			{
				Exception ex = (Exception)t.Exception;
				HandleError(ex);
			} 
			catch {}
		}

		static private void HandleError(Exception ex)
		{
			try
			{
				ExceptionDialog eDlg = new ExceptionDialog(ex);
				eDlg.ShowDialog();
			} 
			catch {}
		}
	}
}
