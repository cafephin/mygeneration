using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScriptControlTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CheckForScriptControl();
        }

        public void CheckForScriptControl()
        {
            string[] keys = new string[] { @"HKEY_CLASSES_ROOT\Wow6432Node\CLSID\{0E59F1D5-1FBE-11D0-8FF2-00A0D10038BC}\InprocServer32", @"HKEY_CLASSES_ROOT\CLSID\{0E59F1D2-1FBE-11D0-8FF2-00A0D10038BC}\InprocServer32" };

            foreach (string key in keys)
            {
                var pathToScriptControl = Registry.GetValue(key, null, string.Empty).ToString();
                if (!string.IsNullOrWhiteSpace(pathToScriptControl))
                {
                    labelKeyName.Text = key;
                    labelKeyValue.Text = pathToScriptControl;
                    break;
                }
            }

        }

        private void buttonExec_Click(object sender, EventArgs e)
        {
            Execute();
        }

        public void Execute()
        {
            try
            {
                MSScriptControl.ScriptControl sc = Activator.CreateInstance(Type.GetTypeFromProgID("MSScriptControl.ScriptControl.1")) as MSScriptControl.ScriptControl;
                sc.Language = "JavaScript";
                sc.AddObject("y", textBoxVariable.Text.Trim(), false);
                sc.AddCode("var jsonVar = " + textBoxJsonVar.Text + ";");
                sc.AddObject("fileInfo", new FileInfo(Assembly.GetExecutingAssembly().Location), true);
                var result = sc.Eval(textBoxCode.Text);
                MessageBox.Show(this, "result is " + result);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.ToString());
            }
        }
    }
}
