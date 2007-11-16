using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MyGenVS2005
{
    public partial class AddInTemplateBrowser : Form
    {
        public AddInTemplateBrowser()
        {
            InitializeComponent();

            this.templateBrowserControl1.Initialize();
        }
    }
}