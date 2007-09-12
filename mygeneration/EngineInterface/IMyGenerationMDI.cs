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
        void OpenDocuments(params string[] filenames);
        void CreateDocument(params string[] args);
        bool IsDocumentOpen(string text, params IMyGenDocument[] docsToExclude);
        IMyGenDocument FindDocument(string text, params IMyGenDocument[] docsToExclude);
        FindForm FindDialog { get; }
        ReplaceForm ReplaceDialog { get; }
        ScintillaConfigureDelegate ConfigureDelegate { get; }
        DockPanel DockPanel { get; }
        void SendAlert(IMyGenContent sender, string command, params object[] args);

        //void WriteConsole(string text, params object[] args);
        //void WriteOutput(string outputLanguage, string text);
        //void ExceptionOccurred(Exception ex);
        //SciTEProperties Properties { get; }
    }
}
