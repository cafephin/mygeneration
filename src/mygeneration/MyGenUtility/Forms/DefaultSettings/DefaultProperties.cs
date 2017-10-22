using System;
using System.Windows.Forms;
using MyMeta;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration
{
    public class DefaultProperties : DockContent, IMyGenDocument
	{
		private ToolStrip _toolStripOptions;
        private ToolStripButton _toolStripButtonSave;
        private ToolStripSeparator _toolStripSeparator1;
        private MenuStrip _menuStripMain;
        private ToolStripMenuItem _fileToolStripMenuItem;
        private ToolStripMenuItem _saveToolStripMenuItem;
        private ToolStripMenuItem _closeToolStripMenuItem;
        private DefaultSettingsControl _defaultSettingsControl;
        private ToolStripSeparator _toolStripMenuItem1;
        private readonly IMyGenerationMDI _mdi;

        public DefaultProperties(IMyGenerationMDI mdi)
		{
			InitializeComponent();

            _mdi = mdi;
            _defaultSettingsControl.ShowOLEDBDialog = new ShowOleDbDialogHandler(ShowOleDbDialog);
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
            this._toolStripOptions = new System.Windows.Forms.ToolStrip();
            this._toolStripButtonSave = new System.Windows.Forms.ToolStripButton();
            this._toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._menuStripMain = new System.Windows.Forms.MenuStrip();
            this._fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this._defaultSettingsControl = new MyGeneration.DefaultSettingsControl();
            this._toolStripOptions.SuspendLayout();
            this._menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // _toolStripOptions
            // 
            this._toolStripOptions.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripButtonSave,
            this._toolStripSeparator1});
            this._toolStripOptions.Location = new System.Drawing.Point(0, 0);
            this._toolStripOptions.Name = "_toolStripOptions";
            this._toolStripOptions.Size = new System.Drawing.Size(896, 25);
            this._toolStripOptions.TabIndex = 34;
            this._toolStripOptions.Visible = false;
            // 
            // _toolStripButtonSave
            // 
            this._toolStripButtonSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._toolStripButtonSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._toolStripButtonSave.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._toolStripButtonSave.MergeIndex = 0;
            this._toolStripButtonSave.Name = "_toolStripButtonSave";
            this._toolStripButtonSave.Size = new System.Drawing.Size(23, 22);
            this._toolStripButtonSave.Text = "Save Settings";
            this._toolStripButtonSave.Click += new System.EventHandler(this.ToolStripButtonSave_OnClicked);
            // 
            // _toolStripSeparator1
            // 
            this._toolStripSeparator1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._toolStripSeparator1.MergeIndex = 1;
            this._toolStripSeparator1.Name = "_toolStripSeparator1";
            this._toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // _menuStripMain
            // 
            this._menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._fileToolStripMenuItem});
            this._menuStripMain.Location = new System.Drawing.Point(0, 0);
            this._menuStripMain.Name = "_menuStripMain";
            this._menuStripMain.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this._menuStripMain.Size = new System.Drawing.Size(896, 24);
            this._menuStripMain.TabIndex = 39;
            this._menuStripMain.Text = "menuStrip1";
            this._menuStripMain.Visible = false;
            // 
            // _fileToolStripMenuItem
            // 
            this._fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._saveToolStripMenuItem,
            this._closeToolStripMenuItem,
            this._toolStripMenuItem1});
            this._fileToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.MatchOnly;
            this._fileToolStripMenuItem.MergeIndex = 0;
            this._fileToolStripMenuItem.Name = "_fileToolStripMenuItem";
            this._fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this._fileToolStripMenuItem.Text = "&File";
            // 
            // _saveToolStripMenuItem
            // 
            this._saveToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._saveToolStripMenuItem.MergeIndex = 4;
            this._saveToolStripMenuItem.Name = "_saveToolStripMenuItem";
            this._saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this._saveToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this._saveToolStripMenuItem.Text = "&Save";
            this._saveToolStripMenuItem.Click += new System.EventHandler(this.SaveToolStripMenuItem_OnClicked);
            // 
            // _closeToolStripMenuItem
            // 
            this._closeToolStripMenuItem.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._closeToolStripMenuItem.MergeIndex = 5;
            this._closeToolStripMenuItem.Name = "_closeToolStripMenuItem";
            this._closeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this._closeToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this._closeToolStripMenuItem.Text = "&Close";
            this._closeToolStripMenuItem.Click += new System.EventHandler(this.CloseToolStripMenuItem_OnClicked);
            // 
            // _toolStripMenuItem1
            // 
            this._toolStripMenuItem1.MergeAction = System.Windows.Forms.MergeAction.Insert;
            this._toolStripMenuItem1.MergeIndex = 6;
            this._toolStripMenuItem1.Name = "_toolStripMenuItem1";
            this._toolStripMenuItem1.Size = new System.Drawing.Size(145, 6);
            // 
            // _defaultSettingsControl
            // 
            this._defaultSettingsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._defaultSettingsControl.Location = new System.Drawing.Point(0, 0);
            this._defaultSettingsControl.MinimumSize = new System.Drawing.Size(601, 574);
            this._defaultSettingsControl.Name = "_defaultSettingsControl";
            this._defaultSettingsControl.Size = new System.Drawing.Size(896, 627);
            this._defaultSettingsControl.TabIndex = 40;
            // 
            // DefaultProperties
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(590, 531);
            this.ClientSize = new System.Drawing.Size(896, 627);
            this.Controls.Add(this._menuStripMain);
            this.Controls.Add(this._defaultSettingsControl);
            this.Controls.Add(this._toolStripOptions);
            this.DockAreas = WeifenLuo.WinFormsUI.Docking.DockAreas.Document;
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DefaultProperties";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TabText = "Default Settings";
            this.Text = "Default Settings";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DefaultProperties_OnClosing);
            this.Load += new System.EventHandler(this.DefaultProperties_OnLoading);
            this.Leave += new System.EventHandler(this.DefaultProperties_OnLeaving);
            this._toolStripOptions.ResumeLayout(false);
            this._toolStripOptions.PerformLayout();
            this._menuStripMain.ResumeLayout(false);
            this._menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void DefaultProperties_OnLoading(object sender, EventArgs e)
        {
            _defaultSettingsControl.Populate();
        }

	    private void DefaultProperties_OnClosing(object sender, FormClosingEventArgs e)
        {
            if ((e.CloseReason != CloseReason.UserClosing) && (e.CloseReason != CloseReason.FormOwnerClosing)) return;

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

        private void SaveToolStripMenuItem_OnClicked(object sender, EventArgs e)
        {
            if (_defaultSettingsControl.Save())
            {
                _mdi.SendAlert(this, "UpdateDefaultSettings");
            }
        }

        private void ToolStripButtonSave_OnClicked(object sender, EventArgs e)
        {
            if (_defaultSettingsControl.Save())
            {
                _mdi.SendAlert(this, "UpdateDefaultSettings");
            }
        }

        private void CloseToolStripMenuItem_OnClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void DefaultProperties_OnLeaving(object sender, EventArgs e)
        {
            var r = DialogResult.None;

            // Something's Changed since the load...
            if (_defaultSettingsControl.SettingsModified)
            {
                r = MessageBox.Show("Default settings have changed.\r\n Would you like to save before leaving?", "Default Settings Changed", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            }
            else if (_defaultSettingsControl.ConnectionInfoModified)
            {
                r = MessageBox.Show("The loaded connection profile has changed.\r\n Would you like to save before leaving?", "Connection Profile Changed", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            }

            if (r == DialogResult.Yes)
            {
                if (_defaultSettingsControl.Save())
                {
                    _mdi.SendAlert(this, "UpdateDefaultSettings");
                }
            }
        }

        #region IMyGenDocument Members

        public ToolStrip ToolStrip
        {
            get { return _toolStripOptions; }
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
