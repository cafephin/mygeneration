using System;
using System.Collections.Generic;
using System.Text;
using Scintilla.Forms;
using Scintilla;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration
{
    public interface IMyGenerationMDI
    {
        FindForm FindDialog { get; }
        ReplaceForm ReplaceDialog { get; }
        ScintillaConfigureDelegate ConfigureDelegate { get; }
        DockPanel DockPanel { get; }
        //void WriteConsole(string text, params object[] args);
        //void WriteOutput(string outputLanguage, string text);
        //void ExceptionOccurred(Exception ex);
        //SciTEProperties Properties { get; }
    }
}
