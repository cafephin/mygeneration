using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace MyGeneration
{
    class Program
    {
        /// <summary>
        /// The main entry point for the application. Set the global application exception handlers here and load up the parent form.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ExceptionHandler.UnhandledExceptions);
            Application.ThreadException += new ThreadExceptionEventHandler(ExceptionHandler.OnThreadException);

            //DialogResult dr = MessageBox.Show("Would you like to try the new UI?", "UI?", MessageBoxButtons.YesNo);
            Form form;
            //if (dr == DialogResult.No)
            //{
            //    form = new MDIParent(Application.StartupPath, args);
            //}
            //else
            //{
                form = new MyGenerationMDI(Application.StartupPath, args);
            //}
            Application.Run(form);
        }

    }
}