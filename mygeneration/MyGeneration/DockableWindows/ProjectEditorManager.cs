using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MyGeneration 
{
    public class ProjectEditorManager : IEditorManager
    {
       // private const string FILE_TYPES = "MyGeneration Project Files (*.zprj)|*.zprj|";
       // private const string FILE_EXTS = ".zprj";
        //private const string NAME = "MyGeneration Project";

        private SortedList<string, string> fileExtensions;
        private List<string> fileTypes;

        public string Name
        {
            get
            {
                return "MyGeneration Project";
            }
        }

        public SortedList<string, string> FileExtensions
        {
            get
            {
                if (fileExtensions == null)
                {
                    fileExtensions = new SortedList<string, string>();
                    fileExtensions.Add("zprj", "MyGeneration Project Files");
                }
                return fileExtensions;
            }
        }

        public List<string> FileTypes
        {
            get
            {
                if (fileTypes == null)
                {
                    fileTypes = new List<string>();
                    fileTypes.Add("MyGeneration Project");;
                }
                return fileTypes;
            }
        }

        public bool CanOpenFile(FileInfo file)
        {
            return FileExtensions.ContainsKey(file.Extension.Trim('.'));
        }

        public IMyGenDocument Open(IMyGenerationMDI mdi, FileInfo file, params string[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public IMyGenDocument Create(IMyGenerationMDI mdi, params string[] args)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
