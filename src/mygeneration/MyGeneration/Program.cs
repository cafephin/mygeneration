using System;
using System.Threading;
using System.Windows.Forms;

namespace MyGeneration
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application. Set the global 
        /// application exception handlers here and load up the parent form.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            MyGenerationMDI mdi;
            try
            {
                mdi = new MyGenerationMDI(Application.StartupPath, args);

                AppDomain.CurrentDomain.UnhandledException += mdi.UnhandledExceptions;
                Application.ThreadException += mdi.OnThreadException;
            }
            catch (Exception ex)
            {
                var exceptionDialog = new MyGeneration.CrazyErrors.ExceptionDialog(ex);
                exceptionDialog.ShowDialog();

                mdi = null;
            }

            if (mdi != null)
            {
                Application.Run(mdi);
            }
            else
            {
                Application.Exit();
            }
        }

    }
}