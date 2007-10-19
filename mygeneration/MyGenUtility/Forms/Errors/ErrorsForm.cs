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
                item.Tag = error;
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

        public IMyGenError SelectedError
        {
            get
            {
                if (this.listView1.SelectedItems.Count == 0) return null;
                return this.listView1.SelectedItems[0].Tag as IMyGenError;
            }
        }

        public List<IMyGenError> SelectedErrors
        {
            get
            {
                List<IMyGenError> errors = new List<IMyGenError>();
                foreach (ListViewItem i in this.listView1.SelectedItems)
                {
                    errors.Add(i.Tag as IMyGenError);
                }
                return errors;
            }
        }

        private string ErrorText
        {
            get
            {
                return GetErrorText(-1);
            }
        }

        private string GetErrorText(int numErrors)
        {
            StringBuilder body = new StringBuilder();
            body.Append("OS Version: ").AppendLine(Environment.OSVersion.VersionString);
            body.Append("EXE Version: ").AppendLine(System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());

            int i = 0;
            foreach (IMyGenError err in this.SelectedErrors)
            {
                body.AppendLine("-----------------------------------------------------------------");
                body.Append(err.ToString());

                if ((numErrors != -1) && (++i >= numErrors)) break;
            }
            return body.ToString();
        }

        private void toolStripButtonEmail_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                string toEmail = "jegreenwo@users.sourceforge.net,thegriftster@users.sourceforge.net";
                string subject = "MyGeneration Error Report";
                string body = GetErrorText(2);
                string message =
                     string.Format("mailto:{0}?subject={1}&body={2}", toEmail, Uri.EscapeUriString(subject), Uri.EscapeUriString(body));
                Process.Start(message);
            }
        }

        private void toolStripButtonSave_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                StringBuilder fname = new StringBuilder();
                fname.Append("ErrorReport_").Append(DateTime.Now.Year)
                    .Append(DateTime.Now.Month.ToString().PadRight(2, '0'))
                    .Append(DateTime.Now.Day.ToString().PadRight(2, '0'))
                    .Append(DateTime.Now.Hour.ToString().PadRight(2, '0'))
                    .Append(DateTime.Now.Minute.ToString().PadRight(2, '0'))
                    .Append(DateTime.Now.Second.ToString().PadRight(2, '0'))
                    .Append(DateTime.Now.Millisecond.ToString().PadRight(2, '0'))
                    .Append(".txt");

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = fname.ToString();
                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    System.IO.File.WriteAllText(sfd.FileName, ErrorText);
                }
            }
        }

        private void toolStripButtonViewDetail_Click(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count > 0)
            {
                this._mdi.PerformMdiFuntion(this, "ShowErrorDetail", this.SelectedErrors);
            }
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            IMyGenError error = SelectedError;
            if (error.Class == MyGenErrorClass.Template)
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    this._mdi.PerformMdiFuntion(this, 
                        "NavigateToTemplateError", this.SelectedError);
                }
            }
            else
            {
                if (this.listView1.SelectedItems.Count > 0)
                {
                    this._mdi.PerformMdiFuntion(this, "ShowErrorDetail", this.SelectedErrors);
                }
            }
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