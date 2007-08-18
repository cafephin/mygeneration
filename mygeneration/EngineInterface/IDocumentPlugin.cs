using System;
using System.Collections.Generic;
using System.Text;
using Scintilla.Forms;
using Scintilla.Configuration;
using Scintilla.Configuration.SciTE;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration
{
    public interface IDocumentPlugin
    {
        Guid PluginID { get; }
        string Name { get; }
        string Description { get; }

        void Initialize(IMyGenerationMDI mdi);
        void Finalize(IMyGenerationMDI mdi);
    }
}
