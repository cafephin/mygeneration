using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Zeus;
using Zeus.UserInterface;
using Zeus.UserInterface.WinForms;
using MyMeta;

namespace MyGeneration
{
    public partial class TemplateBrowser : DockContent, IMyGenContent
    {
        private IMyGenerationMDI _mdi;

        public TemplateBrowser(IMyGenerationMDI mdi)
        {
            this._mdi = mdi;
            this.DockPanel = mdi.DockPanel;

            InitializeComponent();

            this.templateBrowserControl.Initialize(mdi);
        }

        private void templateBrowserControl_TemplateOpen(object sender, EventArgs e)
        {
            if (sender != null)
            {
                this._mdi.OpenDocuments(sender.ToString());
            }
        }

        private void templateBrowserControl_TemplateUpdate(object sender, EventArgs e)
        {
            if (sender != null)
            {
                this._mdi.SendAlert(this, "UpdateTemplate", sender.ToString());
            }
        }

        private void templateBrowserControl_TemplateDelete(object sender, EventArgs e)
        {
            if (sender != null)
            {
                this._mdi.SendAlert(this, "DeleteTemplate", sender.ToString());
            }
        }

        #region IMyGenContent Members

        public ToolStrip ToolStrip
        {
            get { return null; }
        }

        public void ProcessAlert(IMyGenContent sender, string command, params object[] args)
        {
            DefaultSettings settings = DefaultSettings.Instance;
            if (command.Equals("UpdateDefaultSettings", StringComparison.CurrentCultureIgnoreCase))
            {
                bool doRefresh = false;

                try
                {
                    if (this.templateBrowserControl.TreeBuilder.DefaultTemplatePath != settings.DefaultTemplateDirectory)
                    {
                        doRefresh = true;
                    }
                }
                catch
                {
                    doRefresh = true;
                }

                if (doRefresh)
                    templateBrowserControl.RefreshTree();
            }
        }

        public bool CanClose(bool allowPrevent)
        {
            return true;
        }

        public DockContent DockContent
        {
            get { return this; }
        }

        #endregion
    }
}