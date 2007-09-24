using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration.Forms
{
    public partial class ConsoleForm : DockContent, IMyGenConsole
    {
        private IMyGenerationMDI mdi;
        private ZeusScintillaControl zcs;

        public ConsoleForm(IMyGenerationMDI mdi)
        {
            this.mdi = mdi;
            InitializeComponent();

            zcs = new ZeusScintillaControl();
            zcs.BringToFront();
            zcs.Dock = DockStyle.Fill;
            zcs.Name = "test";
            this.Controls.Add(zcs);
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {

        }

        #region IMyGenConsole Members

        public ToolStrip ToolStrip
        {
            get { return null; }
        }

        public void ProcessAlert(IMyGenContent sender, string command, params object[] args)
        {
            //
        }

        public bool CanClose(bool allowPrevent)
        {
            return true;
        }

        public DockContent DockContent
        {
            get { return this; }
        }

        public void Write(string text)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(DateTime.Now);
            sb.Append(" - ");
            sb.Append(text);
            if (!text.EndsWith(Environment.NewLine)) sb.Append(Environment.NewLine);

            this.zcs.AppendText(sb.ToString());
            this.zcs.ScrollCaret();
        }

        public void Write(string text, params object[] args)
        {
            Write(string.Format(text, args));
        }

        public void Write(Exception ex)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(ex.GetType().Name)
            .Append(": ")
            .Append(ex.Message)
            .Append(" - ")
            .Append(ex.StackTrace);

            Write(sb.ToString());
        }

        #endregion
    }
}