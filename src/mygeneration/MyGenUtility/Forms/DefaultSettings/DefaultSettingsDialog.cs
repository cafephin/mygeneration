using System;
using System.Windows.Forms;
using MyMeta;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration
{
    public class DefaultSettingsDialog : DockContent, IMyGenDocument
	{
		private DefaultSettingsControl _defaultSettingsControl;
        private readonly IMyGenerationMDI _mdi;

        public DefaultSettingsDialog(IMyGenerationMDI mdi)
		{
			InitializeComponent();

            _mdi = mdi;
            _defaultSettingsControl.ShowOleDbDialog = new ShowOleDbDialogHandler(ShowOleDbDialog);
            _defaultSettingsControl.Initialize(mdi);
		}

	    private string ShowOleDbDialog(string cs)
        {
            return _mdi.PerformMdiFuntion(this, "ShowOLEDBDialog", cs) as string; 
        }

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this._defaultSettingsControl = new MyGeneration.DefaultSettingsControl();
            this.SuspendLayout();
            // 
            // _defaultSettingsControl
            // 
            this._defaultSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._defaultSettingsControl.Location = new System.Drawing.Point(0, 0);
            this._defaultSettingsControl.MinimumSize = new System.Drawing.Size(601, 574);
            this._defaultSettingsControl.Name = "_defaultSettingsControl";
            this._defaultSettingsControl.Size = new System.Drawing.Size(601, 650);
            this._defaultSettingsControl.TabIndex = 40;
            // 
            // DefaultSettingsDialog
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(590, 531);
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(592, 650);
            this.ControlBox = false;
            this.Controls.Add(this._defaultSettingsControl);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DefaultSettingsDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TabText = "Default Settings";
            this.Text = "Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DefaultProperties_OnClosing);
            this.Load += new System.EventHandler(this.DefaultProperties_OnLoading);
            this.ResumeLayout(false);

		}
		#endregion

        private void DefaultProperties_OnLoading(object sender, EventArgs e)
        {
            _defaultSettingsControl.Populate();
        }

	    private void DefaultProperties_OnClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing && e.CloseReason != CloseReason.FormOwnerClosing) return;

            var dialogResult = DialogResult.None;

            if (_defaultSettingsControl.SettingsModified)
            {
                dialogResult = MessageBox.Show("Default settings have changed.\r\n Would you like to save before exiting?",
                                               "Default Settings Changed",
                                               MessageBoxButtons.YesNoCancel,
                                               MessageBoxIcon.Exclamation);
            }
            else if (_defaultSettingsControl.ConnectionInfoModified)
            {
                dialogResult = MessageBox.Show("The loaded connection profile has changed.\r\n Would you like to save before exiting?",
                                               "Connection Profile Changed",
                                               MessageBoxButtons.YesNoCancel,
                                               MessageBoxIcon.Exclamation);
            }

            if (dialogResult == DialogResult.None) return;

            if (dialogResult == DialogResult.Cancel)
            {
                e.Cancel = true;
            }
            else if (dialogResult == DialogResult.Yes)
            {
                if (!_defaultSettingsControl.Save()) return;

                DialogResult = DialogResult.OK;
                _mdi.SendAlert(this, "UpdateDefaultSettings");
            }
            else
            {
                _defaultSettingsControl.Cancel();
                DialogResult = DialogResult.Cancel;
            }
        }

        #region IMyGenDocument Members

        public ToolStrip ToolStrip
        {
            get { return null; }
        }

        public string DocumentIndentity
        {
            get { return "::DefaultSettings::"; }
        }

        public void ProcessAlert(IMyGenContent sender, string command, params object[] args)
        {
            //throw new Exception("The method or operation is not implemented.");
        }

        public bool CanClose(bool allowPrevent)
        {
            return true;
        }

        public DockContent DockContent
        {
            get { return this; }
        }

        public string TextContent
        {
            get { return _defaultSettingsControl.TextContent; }
        }

        #endregion
    }
}
