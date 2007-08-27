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

        #region Browser launch code
        public static void LaunchBrowser(string url)
        {
            try
            {
                Process p = new Process();
                p.StartInfo.FileName = DefaultBrowser;
                p.StartInfo.Arguments = url;
                p.Start();
            }
            catch
            {
                System.Diagnostics.Process.Start(url);
            }
        }

        public static void LaunchBrowser(string url, ProcessWindowStyle windowStyle, bool createNoWindow)
        {
            try
            {
                try
                {
                    Process p = new Process();
                    p.StartInfo.FileName = DefaultBrowser;
                    p.StartInfo.Arguments = url;
                    p.StartInfo.CreateNoWindow = createNoWindow;
                    p.StartInfo.WindowStyle = windowStyle;
                    p.Start();
                }
                catch { }
            }
            catch
            {
                try
                {
                    Process p = new Process();
                    p.StartInfo.FileName = url;
                    p.StartInfo.CreateNoWindow = createNoWindow;
                    p.StartInfo.WindowStyle = windowStyle;
                    p.Start();
                }
                catch { }
            }
        }

        private static string s_browser = string.Empty;

        private static string DefaultBrowser
        {
            get
            {
                if (s_browser == string.Empty)
                {
                    s_browser = getDefaultBrowser();
                }
                return s_browser;
            }
        }

        private static string getDefaultBrowser()
        {
            string browser = string.Empty;
            RegistryKey key = null;
            try
            {
                key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false);

                //trim off quotes
                browser = key.GetValue(null).ToString().ToLower().Replace("\"", "");
                if (!browser.EndsWith("exe"))
                {
                    //get rid of everything after the ".exe"
                    browser = browser.Substring(0, browser.LastIndexOf(".exe") + 4);
                }
            }
            finally
            {
                if (key != null) key.Close();
            }
            return browser;
        }
        #endregion
    }
}