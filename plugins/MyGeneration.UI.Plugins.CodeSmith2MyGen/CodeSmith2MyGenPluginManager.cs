using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using MyGeneration;

namespace MyGeneration.UI.Plugins.CodeSmith2MyGen
{
    public class CodeSmith2MyGenPluginManager : ISimplePluginManager
    {
        public string Name
        {
            get { return "CodeSmith2MyGen Plugin"; }
        }

        public string Description
        {
            get { return ". - komma8.komma1"; }
        }

        public Uri AuthorUri
        {
            get
            {
                return new Uri("http://sourceforge.net/projects/mygeneration/");
            }
        }

        public bool AddToolbarIcon
        {
            get { return true; }
        }

        public Image MenuImage
        {
            get { return null; }
        }

        public void Execute(IMyGenerationMDI mdi, params string[] args)
        {
            /*SampleDialogContent sdc = new SampleDialogContent(mdi);
            DialogResult result = sdc.ShowDialog(mdi.DockPanel.Parent.FindForm());*/
        }
    }
}
