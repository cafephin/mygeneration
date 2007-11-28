using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace MyGenVS2005
{
    public partial class AddInErrorForm : Form
    {
        private List<Exception> _exceptions = new List<Exception>();
        private int _idx = 0;

        public AddInErrorForm()
        {
            InitializeComponent();
        }

        public AddInErrorForm(params Exception[] exs)
        {
            InitializeComponent();
        }

        public List<Exception> Exceptions
        {
            get { return _exceptions; }
            set { _exceptions = value; }
        }

        private void AddInErrorForm_Load(object sender, EventArgs e)
        {
            Update();
        }

        private void Update()
        {
            bool vis = (_exceptions.Count > 1);

            this.buttonPrevious.Enabled = vis;
            this.buttonNext.Enabled = vis;

            if (_exceptions.Count > 0)
            {
                List<MyGeneration.IMyGenError> errors = MyGeneration.MyGenError.CreateErrors(_exceptions[_idx]);
                this.errorDetailControl1.Update(errors[0]);
            }
        }

        private void buttonNext_Click(object sender, EventArgs e)
        {
            if (_exceptions.Count > 0)
            {
                _idx++;
                if (_idx >= _exceptions.Count) _idx = 0;
                Update();
            }
        }

        private void buttonPrevious_Click(object sender, EventArgs e)
        {
            if (_exceptions.Count > 0)
            {
                _idx--;
                if (_idx < 0) _idx = _exceptions.Count - 1;
                Update();
            }
        }
    }
}