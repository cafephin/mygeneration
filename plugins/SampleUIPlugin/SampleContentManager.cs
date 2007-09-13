using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;
using MyGeneration;

namespace SampleUIPlugin
{
    public class SampleContentManager : IContentManager
    {
        public string Name
        {
            get { return "Sample Content"; }
        }

        public Image MenuImage
        {
            get { return Properties.Resources.bgb; }
        }

        public IMyGenContent Create(IMyGenerationMDI mdi, params string[] args)
        {
            return new SampleContent(mdi);
        }
    }
}
