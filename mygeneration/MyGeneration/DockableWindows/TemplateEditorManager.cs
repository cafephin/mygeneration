using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Zeus;

namespace MyGeneration
{
    public class TemplateEditorManager : IEditorManager
    {
        //public const string FILE_TYPES = "JScript Templates (*.jgen)|*.jgen|VBScript Templates (*.vbgen)|*.vbgen|C# Templates (*.csgen)|*.csgen|Zeus Templates (*.zeus)|*.zeus|";
        //public const string FILE_EXTS = ".jgen.vbgen.csgen.zeus";
        //public const string NAME = "Zeus Template Editor";
        //public const string NAME = "Zeus Template Editor";

        private SortedList<string, string> fileExtensions;
        private List<string> fileTypes;

        public string Name
        {
            get
            {
                return "Zeus Template Editor";
            }
        }

        public SortedList<string, string> FileExtensions
        {
            get
            {
                if (fileExtensions == null)
                {
                    fileExtensions = new SortedList<string, string>();
                    fileExtensions.Add("jgen", "JScript Zeus Templates");
                    fileExtensions.Add("vbgen", "VBScript Zeus Templates");
                    fileExtensions.Add("csgen", "C# Zeus Templates");
                    fileExtensions.Add("zeus", "Zeus Templates");
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
                    fileTypes.Add("JScript Zeus Template");
                    fileTypes.Add("VBScript Zeus Template");
                    fileTypes.Add("C# Zeus Template");
                    fileTypes.Add("VB.Net Template");
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
            TemplateEditor edit = null;

            if (file.Exists)
            {
                bool isopen = mdi.IsDocumentOpen(file.FullName);

                if (!isopen)
                {
                    edit = new TemplateEditor(mdi);
                    edit.FileOpen(file.FullName);
                }
                else
                {
                    edit = mdi.FindDocument(file.FullName) as TemplateEditor;
                    if (edit != null)
                    {
                        edit.Activate();
                    }
                }
            }

            return edit;
        }

        public IMyGenDocument Create(IMyGenerationMDI mdi, params string[] args)
        {
            TemplateEditor edit = new TemplateEditor(mdi);

            switch (args[0])
            {
                case "C# Zeus Template":
                    edit.FileNew("ENGINE", ZeusConstants.Engines.DOT_NET_SCRIPT, "LANGUAGE", ZeusConstants.Languages.CSHARP);
                    break;
                case "VB.Net Template":
                    edit.FileNew("ENGINE", ZeusConstants.Engines.DOT_NET_SCRIPT, "LANGUAGE", ZeusConstants.Languages.VBNET);
                    break;
                case "VBScript Zeus Template":
                    edit.FileNew("ENGINE", ZeusConstants.Engines.MICROSOFT_SCRIPT, "LANGUAGE", ZeusConstants.Languages.VBSCRIPT);
                    break;
                case "JScript Zeus Template":
                default:
                    edit.FileNew("ENGINE", ZeusConstants.Engines.MICROSOFT_SCRIPT, "LANGUAGE", ZeusConstants.Languages.JSCRIPT);
                    break;
            }

            return edit;
        }
    }
}
