using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MyGeneration.Forms
{
    public partial class ErrorDetailControl : UserControl
    {
        public ErrorDetailControl()
        {
            InitializeComponent();
        }

        public void Initialize()
        {
        }

        public string TextContent
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
