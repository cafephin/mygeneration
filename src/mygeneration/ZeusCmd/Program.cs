using System;

namespace ZeusCmd
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static int Main(string[] args)
        {
            var cmd = new Zeus.ZeusCmd(args);
            return cmd.ReturnValue;
        }
    }
}
