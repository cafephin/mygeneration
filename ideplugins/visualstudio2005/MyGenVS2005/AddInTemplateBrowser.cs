using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.CommandBars;

using Zeus;
using Zeus.Configuration;

namespace MyGenVS2005
{
    public partial class AddInTemplateBrowser : Form
    {
        private DTE2 _application;

        public AddInTemplateBrowser(DTE2 application)
        {
            _application = application;

            InitializeComponent();

            this.templateBrowserControl1.Initialize();
        }

        private void templateBrowserControl1_ErrorsOccurred(object sender, EventArgs e)
        {
            if (sender is Exception)
            {
                AddInErrorForm errorForm = new AddInErrorForm(sender as Exception);
                errorForm.ShowDialog(this.ParentForm);
            }
        }

        private void templateBrowserControl1_TemplateOpen(object sender, EventArgs e)
        {
            if (this.checkBoxOpenTemplate.Checked)
            {
                String path = sender as String;
                if (!string.IsNullOrEmpty(path))
                {
                    _application.ItemOperations.OpenFile(path, EnvDTE.Constants.vsViewKindPrimary);
                }
            }
        }

        private void templateBrowserControl1_GeneratedFileSaved(object sender, EventArgs e)
        {

            if (this.checkBoxOpenFile.Checked)
            {
                String path = sender as String;
                if (!string.IsNullOrEmpty(path))
                {
                    _application.ItemOperations.OpenFile(path, EnvDTE.Constants.vsViewKindPrimary);
                }
            }
        }
    }
}