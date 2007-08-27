using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyGeneration
{
    public interface IEditorManager
    {
        string Name { get; }
        SortedList<string, string> FileExtensions { get; }
        List<string> FileTypes { get; }
        bool CanOpenFile(FileInfo file);
        IMyGenDocument Open(IMyGenerationMDI mdi, FileInfo file, params string[] args);
        IMyGenDocument Create(IMyGenerationMDI mdi, params string[] args);
    }
}
