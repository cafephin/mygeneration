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
        private IMyGenerationMDI _mdi;
        private List<IMyGenError> _errors = new List<IMyGenError>();

        public ErrorsForm(IMyGenerationMDI mdi)
        {
            this._mdi = mdi;
            InitializeComponent();
        }

        private void AddError(IMyGenError error)
        {
            if (_errors.Count == 0) _errors.Add(error); 
            else _errors.Insert(0, error);
        }

        private void BindErrors()
        {
            this.listView1.Items.Clear();
            foreach (IMyGenError error in _errors)
            {
                ListViewItem item = new ListViewItem(error.DateTimeOccurred.ToString());
                item.SubItems.Add(error.Class.ToString());
                item.SubItems.Add(error.Message);
                item.SubItems.Add(error.Detail);
                item.Tag = item;
                this.listView1.Items.Add(item);
            }
            Application.DoEvents();
            if (this.IsHidden)
            {
                this.Show(this._mdi.DockPanel);
                this.VisibleState = DockState.DockBottomAutoHide;
            }

            this.Activate();
            this.Refresh();
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
            AddErrors(MyGenError.CreateErrors(exceptions).ToArray());
            //throw new Exception("The method or operation is not implemented.");
        }

        public void AddErrors(params IMyGenError[] errors)
        {
            _errors.AddRange(errors);
            BindErrors();
        }

        public List<IMyGenError> Errors
        {
            get
            { //throw new Exception("The method or operation is not implemented."); }
                return _errors;
            }
        }

        #endregion
    }
}