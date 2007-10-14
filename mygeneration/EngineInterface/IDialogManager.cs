using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;

namespace MyGeneration
{
    public interface IDialogManager
    {
        string Name { get; }
        string Description { get; }
        Uri AuthorUri { get; }
        Image MenuImage { get; }
        IMyGenDialog Create(IMyGenerationMDI mdi, params string[] args);
    }
}
