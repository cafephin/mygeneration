using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration.Forms
{
    public partial class ErrorsForm : DockContent, IMyGenErrorList
    {
        private IMyGenerationMDI mdi;

        public ErrorsForm(IMyGenerationMDI mdi)
        {
            this.mdi = mdi;
            InitializeComponent();
        }

        #region IMyGenErrorList Members

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

        public void AddErrors(params Exception[] exceptions)
        {
            foreach (Exception ex in exceptions)
            {
            }
            //throw new Exception("The method or operation is not implemented.");
        }

        public void AddErrors(params IMyGenError[] errors)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public List<IMyGenError> Errors
        {
            get
            { //throw new Exception("The method or operation is not implemented."); }
                return new List<IMyGenError>();
            }
        }

        #endregion
    }
}