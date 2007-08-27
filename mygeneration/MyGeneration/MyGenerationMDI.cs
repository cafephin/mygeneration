using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;

using Scintilla;
using Scintilla.Enums;
using Scintilla.Forms;
using Scintilla.Printing;
using Scintilla.Configuration.Legacy;
using Scintilla.Configuration;
using Scintilla.Configuration.SciTE;

using Zeus.Projects;
using Zeus;

namespace MyGeneration
{
    public partial class MyGenerationMDI : Form, IMyGenerationMDI
    {
        private const string DOCK_CONFIG_FILE = @"\settings\dock.config";
        private const string SCINTILLA_CONFIG_FILE = @"\settings\scintillanet.xml";
        private const string REPLACEMENT_SUFFIX = "$REPLACEMENT$.dll";
        private const string PROJECT_FILE_TYPES = "Zeus Project (*.zprj)|*.zprj|";

        private const string URL_HOMEPAGE = "http://www.mygenerationsoftware.com/home/";
        private const string URL_DOCUMENTATION = "http://www.mygenerationsoftware.com/Documentation.aspx";
        private const string URL_LATESTVERSION = "http://www.mygenerationsoftware.com/LatestVersion";

        private static ConfigFile configFile;
        private ScintillaConfigureDelegate configureDelegate;

        
        private FindForm findDialog = new FindForm();
        private ReplaceForm replaceDialog = new ReplaceForm();
        
        private DefaultSettings settings;
        private LanguageMappings languageMappings;
        private DbTargetMappings dbTargetMappings;
        private MetaDataBrowser metaDataBrowser;
        private UserMetaData userMetaData;
        private GlobalUserMetaData globalUserMetaData;
        private MetaProperties metaProperties;
        private DefaultProperties options;
        private TemplateBrowser templateBrowser;

        private Dictionary<string, IEditorManager> editorManagers = new Dictionary<string,IEditorManager>();

        private string openFileDialogString = "All files (*.*)|*.*";
        private string startupPath;
        private string[] startupFiles;

        public MyGenerationMDI(string startupPath, params string[] args)
        {
            this.startupPath = startupPath;
            
            settings = DefaultSettings.Instance;

            //Any files that were locked when the TemplateLibrary downloaded and tried to replace them will be replaced now.
            ProcessReplacementFiles();

            InitializeComponent();

            //this.IsMdiContainer = true;
            //this.MdiChildActivate += new EventHandler(this.MDIChildActivated);

            startupFiles = args;

            // Open File Dialog Setup
            StringBuilder dialogString = new StringBuilder();
            IEditorManager editorManager = new TemplateEditorManager();
            if (editorManager.FileExtensions.Count > 0)
            {
                dialogString.Append("All MyGeneration Files (");
                StringBuilder exts = new StringBuilder();
                foreach (string ext in editorManager.FileExtensions.Keys)
                {
                    if (exts.Length == 0)
                        exts.AppendFormat("*.{0}", ext);
                    else
                        exts.AppendFormat(";*.{0}", ext);
                }
                dialogString.Append(exts).Append(")|").Append(exts).Append("|");

                foreach (string ext in editorManager.FileExtensions.Keys)
                    dialogString.AppendFormat("{0} (*.{1})|*.{1}|", editorManager.FileExtensions[ext], ext);
            }
            dialogString.Append("All files (*.*)|*.*");

            openFileDialogString = dialogString.ToString();
            editorManagers[editorManager.Name] = editorManager;

            // New File Menu Setup
            foreach (string ftype in editorManager.FileTypes)
            {
                ToolStripMenuItem i = new ToolStripMenuItem(ftype, null, newFileDynamicToolStripMenuItem_Click);
                this.newToolStripMenuItem.DropDownItems.Add(i);

                ToolStripMenuItem ti = new ToolStripMenuItem(ftype, null, newFileDynamicToolStripMenuItem_Click);
                toolStripDropDownButtonNew.DropDownItems.Add(ti);
            }

            //editorManager = new ProjectBrowser();
            //openFileDialogString = editorManager.OpenFileDialogString + openFileDialogString;
            //editorManagers[editorManager.Name] = editorManager;

            this.RefreshRecentFiles();
        }

        private void MyGenerationMDI_Load(object sender, EventArgs e)
        {
            switch (settings.WindowState)
            {
                case "Maximized":

                    this.WindowState = FormWindowState.Maximized;
                    break;

                case "Minimized":

                    this.WindowState = FormWindowState.Minimized;
                    break;

                case "Normal":

                    int x = Convert.ToInt32(settings.WindowPosLeft);
                    int y = Convert.ToInt32(settings.WindowPosTop);
                    int w = Convert.ToInt32(settings.WindowPosWidth);
                    int h = Convert.ToInt32(settings.WindowPosHeight);

                    this.Location = new System.Drawing.Point(x, y);
                    this.Size = new Size(w, h);
                    break;
            }

            // Load up the scintilla configuration
            ConfigurationUtility cu = new ConfigurationUtility();

            FileInfo scintillaConfigFile = new FileInfo(startupPath + SCINTILLA_CONFIG_FILE);
            #region HACK: this needs to be cleaned up at some point.
            // If the file doesn't exist, create it.
            if (scintillaConfigFile.Exists)
            {
                //TODO: Retry this with a copy of the file until we can upgrade Scintilla.Net with a fix.
                int maxTries = 3;
                while (maxTries > 0)
                {
                    try
                    {
                        configFile = cu.LoadConfiguration(scintillaConfigFile.FullName) as ConfigFile;
                        break;
                    }
                    catch
                    {
                        if (--maxTries == 1)
                        {
                            File.Copy(scintillaConfigFile.FullName, scintillaConfigFile.FullName + ".tmp", true);
                            scintillaConfigFile = new FileInfo(scintillaConfigFile.FullName + ".tmp");
                        }
                        else
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                    }
                }
            }
            #endregion
            if (configFile != null)
            {
                configureDelegate = configFile.MasterScintilla.Configure; 
                ZeusScintillaControl.StaticConfigure = configureDelegate;
            }

            // Dock Content configuration
            DeserializeDockContent deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            string dockConfigFileName = startupPath + DOCK_CONFIG_FILE;

            if (File.Exists(dockConfigFileName))
            {
                //dockPanel.LoadFromXml(dockConfigFileName, deserializeDockContent);
            }

            // Startup files from the command line
            if (this.startupFiles != null)
            {
                OpenDocuments(startupFiles);
            }

            // Show Default Properties if this is the first load.
            if (settings.FirstLoad)
            {
                OptionsDockContent.ShowDialog(this);
            }
        }

        private void PickFiles()
        {
            DefaultSettings settings = DefaultSettings.Instance;

            Stream myStream;
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Directory.GetCurrentDirectory();
            openFileDialog.Filter = this.openFileDialogString;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.Multiselect = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                this.OpenDocuments(openFileDialog.FileNames);
            }
        }

        public void CreateDocument(params string[] args)
        {
            if (args.Length == 1)
            {
                foreach (IEditorManager manager in this.editorManagers.Values)
                {
                    if (manager.FileTypes.Contains(args[0]))
                    {
                        IMyGenDocument mygenDoc = manager.Create(this, args);
                        if (mygenDoc != null)
                        {
                            mygenDoc.DockContent.Show(dockPanel);
                        }
                        break;
                    }
                }
            }
        }

        public void OpenDocuments(params string[] filenames)
        {
            foreach (string file in filenames)
            {
                FileInfo info = new FileInfo(file);
                
                if (info.Exists)
                {
                    if (this.IsDocumentOpen(info.FullName))
                    {
                        this.FindDocument(info.FullName).DockContent.Activate();
                    }
                    else
                    {
                        bool isOpened = false;
                        foreach (IEditorManager manager in this.editorManagers.Values)
                        {
                            if (manager.CanOpenFile(info))
                            {
                                IMyGenDocument mygenDoc = manager.Open(this, info);
                                if (mygenDoc != null)
                                {
                                    isOpened = true;
                                    mygenDoc.DockContent.Show(dockPanel);
                                    this.AddRecentFile(info.FullName);
                                }
                                break;
                            }
                        }

                        if (!isOpened)
                        {
                            MessageBox.Show(this, string.Format("Unknown file type in file {0}", info.Name), "Unknown file type");
                        }
                    }
                }
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            IDockContent document = null;
            string[] parsedStrings = persistString.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            /*if (parsedStrings.Length != 3)
            {
                document = null;
            }
            else if (parsedStrings[0] == typeof(TemplateEditor).ToString())
            {
                document = OpenTemplateEditor(parsedStrings[1]);
            }*/

            return document;
        }


        private void dockPanel_ActivePaneChanged(object sender, EventArgs e)
        {
        }

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            
            IDockContent fe = this.dockPanel.ActiveContent as IDockContent;
            if (fe is IEditControl)
            {
                ToolStripManager.RevertMerge(toolStrip1);
                //this.FindDialog.Close();
                //this.ReplaceDialog.Close();
            }
            else if (fe is IMyGenDocument)
            {
                ToolStripManager.RevertMerge(toolStrip1);
                IMyGenDocument mgd = fe as IMyGenDocument;
                if (mgd.ToolStrip != null)
                {
                    ToolStripManager.Merge(mgd.ToolStrip, this.toolStrip1);
                }
            }
        }

        private void dockPanel_ActiveDocumentChanged(object sender, EventArgs e)
        {
        }

        private void MyGenerationMDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            //TemplateEditor editor;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                {
                    /*if (form is TemplateEditor)
                    {
                        editor = form as TemplateEditor;
                        if (!editor.SaveBeforeCloseCheck)
                        {
                            e.Cancel = true;
                            return;
                        }
                    }
                
                    // Close and hidden windows.
                    if (form.Visible == false)
                    {
                        form.Close();
                    }*/
                }
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                {
                    /*if (content is TemplateEditor)
                    {
                        editor = content as TemplateEditor;
                        if (!editor.SaveBeforeCloseCheck)
                        {
                            e.Cancel = true;
                            return;
                        }

                    }
                
                    // Close and hidden windows.
                    if (content.DockHandler.IsHidden)
                    {
                        content.DockHandler.VisibleState = DockState.Hidden;
                        content.DockHandler.Close();
                    }*/
                }
            }

            string configFile = startupPath + DOCK_CONFIG_FILE;
            dockPanel.SaveAsXml(configFile);
        }

        #region DockManager Helper Methods
        public bool IsDocumentOpen(string text, params IMyGenDocument[] docsToExclude)
        {
            IMyGenDocument doc = this.FindDocument(text, docsToExclude);
            return (doc != null);
        }

        public IMyGenDocument FindDocument(string text, params IMyGenDocument[] docsToExclude)
        {
            IMyGenDocument found = null;
            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                {
                    if (form is IMyGenDocument)
                    {
                        IMyGenDocument doc = form as IMyGenDocument;
                        if (doc.DocumentIndentity == text)
                        {
                            foreach (IMyGenDocument exclude in docsToExclude)
                            {
                                if (exclude == doc) doc = null;
                            }
                            if (doc != null)
                            {
                                found = doc;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                {
                    if (content is IMyGenDocument)
                    {
                        IMyGenDocument doc = content as IMyGenDocument;
                        if (doc.DocumentIndentity == text)
                        {
                            foreach (IMyGenDocument exclude in docsToExclude)
                            {
                                if (exclude == doc) doc = null;
                            }
                            if (doc != null)
                            {
                                found = doc;
                                break;
                            }
                        }
                    }
                }
            }

            return found;
        }
        #endregion

        /*
        #region Template Editor members
        public enum TemplateKeyType 
        {
            FileName = 0,
            UniqueID
        }

        private TemplateEditor OpenTemplateEditor(string engine, string language)
        {
            TemplateEditor template = new TemplateEditor(this);
            template.FileNew("ENGINE", engine, "LANGUAGE", language);
            template.Show(dockPanel);
            return template;
        }

        private TemplateEditor OpenTemplateEditor()
        {
            TemplateEditor edit = new TemplateEditor(this);
            edit.FileNew("ENGINE", ZeusConstants.Engines.DOT_NET_SCRIPT, "LANGUAGE", ZeusConstants.Languages.CSHARP);
            edit.Show(dockPanel);
            return edit;
        }

        private TemplateEditor OpenTemplateEditor(string filename)
        {
            TemplateEditor edit = null;

            if (File.Exists(filename))
            {
                bool isopen = IsTemplateOpen(filename);

                if (!isopen)
                {
                    edit = new TemplateEditor(this);
                    edit.FileOpen(filename);
                    edit.Show(dockPanel);
                }
                else
                {
                    edit = GetTemplateEditor(filename);
                    edit.Activate();
                }
            }
            else
            {
                edit = OpenTemplateEditor();
            }

            return edit;
        }

        private void CloseTemplateEditor(string filename)
        {
            TemplateEditor editor = this.GetTemplateEditor(TemplateKeyType.FileName, filename, null);
            if (editor != null) editor.Close();
        }

        private void RefreshTemplateEditor(string uniqueId)
        {
            TemplateEditor editor = this.GetTemplateEditor(TemplateKeyType.UniqueID, uniqueId, null);
            if (editor != null)
            {
                if (MessageBox.Show(this,
                    "Template [" + editor.Title + "] has been updated outside of the Template Editor.\r\nWould you like to refresh it?",
                    "Refresh Updated Template?",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    string filename = editor.CompleteFilePath;
                    editor.Close();

                    if (File.Exists(filename))
                    {
                        editor = new TemplateEditor(this);
                        editor.FileOpen(filename);
                        editor.Show(dockPanel);
                    }
                }
            }
        }

        public bool IsTemplateOpen(string filename)
        {
            return IsTemplateOpen(filename, null);
        }

        public bool IsTemplateOpen(string filename, TemplateEditor exclude)
        {
            TemplateEditor editor = GetTemplateEditor(TemplateKeyType.FileName, filename, exclude);
            return (editor != null);
        }

        public TemplateEditor GetTemplateEditor(string filename)
        {
            return GetTemplateEditor(TemplateKeyType.FileName, filename, null);
        }

        public TemplateEditor GetTemplateEditor(TemplateKeyType keyType, string varkey, TemplateEditor exclude)
        {
            string matchingKey, key = varkey;
            FileInfo inf = null;
            if (keyType == TemplateKeyType.FileName)
            {
                inf = new FileInfo(filename);
                key = inf.FullName;
            }

            TemplateEditor editor = null;

            if (dockPanel.DocumentStyle == DocumentStyle.SystemMdi)
            {
                foreach (Form form in MdiChildren)
                {
                    if ((form is TemplateEditor) && (form != exclude))
                    {
                        editor = form as TemplateEditor;
                        matchingKey = (keyType == TemplateKeyType.FileName) ? editor.CompleteFilePath : editor.UniqueID;
                        if (key.Equals(matchingKey, StringComparison.CurrentCultureIgnoreCase)) break;
                        else editor = null;
                    }
                }
            }
            else
            {
                foreach (IDockContent content in dockPanel.Documents)
                {
                    if ((content is TemplateEditor) && (content != exclude))
                    {
                        editor = content as TemplateEditor;
                        matchingKey = (keyType == TemplateKeyType.FileName) ? editor.CompleteFilePath : editor.UniqueID;
                        if (key.Equals(matchingKey, StringComparison.CurrentCultureIgnoreCase)) break;
                        else editor = null;
                    }
                }
            }

            return editor;
        }
        #endregion
*/
        #region Menu Events
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PickFiles();
        }

        private void newFileDynamicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = sender as ToolStripMenuItem;
            this.CreateDocument(i.Text);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewAbout ab = new NewAbout();
            ab.ShowDialog(this);
        }

        private void jScriptTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDocument(ZeusConstants.Engines.MICROSOFT_SCRIPT, ZeusConstants.Languages.JSCRIPT);
        }

        private void vBScriptTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDocument(ZeusConstants.Engines.MICROSOFT_SCRIPT, ZeusConstants.Languages.VBSCRIPT);
        }

        private void cTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDocument(ZeusConstants.Engines.DOT_NET_SCRIPT, ZeusConstants.Languages.CSHARP);
        }

        private void vBNetTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateDocument(ZeusConstants.Engines.DOT_NET_SCRIPT, ZeusConstants.Languages.VBNET);
        }

        private void projectToolStripMenuItem1_Click(object sender, EventArgs e)
        {
        }

        private void defaultSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.OptionsDockContent.IsHidden)
            {
                this.OptionsDockContent.Show(this.dockPanel);
            }
            else
            {
                this.OptionsDockContent.Activate();
            }
        }

        private void contentsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void recentFilesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem)
            {
                ToolStripMenuItem item = sender as ToolStripMenuItem;
                string file = item.Text;
                if (File.Exists(file))
                {
                    AddRecentFile(item.Text);
                    OpenDocuments(item.Text);
                }
                else
                {
                    // May want to add text to resource file for internationalization
                    MessageBox.Show(this, string.Format("The file \"{0}\" no longer exists.", item.Text), "File Missing"); 
                    RemoveRecentFile(item.Text);
                }
            }
        }
        #endregion

        #region Toolstrip Button Events
        private void toolStripButtonTemplateBrowser_Click(object sender, EventArgs e)
        {
            if (this.TemplateBrowserDockContent.IsHidden)
            {
                this.TemplateBrowserDockContent.Show(this.dockPanel);
            }
            else
            {
                this.TemplateBrowserDockContent.Hide();
            }
        }

        private void toolStripButtonOptions_Click(object sender, EventArgs e)
        {
            if (this.OptionsDockContent.IsHidden)
            {
                this.OptionsDockContent.Show(this.dockPanel);
            }
            else
            {
                this.OptionsDockContent.Hide();
            }
        }

        private void toolStripButtonMyMetaBrowser_Click(object sender, EventArgs e)
        {
            if (this.MetaDataBrowserDockContent.IsHidden)
            {
                this.MetaDataBrowserDockContent.Show(this.dockPanel);
            }
            else
            {
                this.MetaDataBrowserDockContent.Hide();
            }
        }

        private void toolStripButtonMyMetaProperties_Click(object sender, EventArgs e)
        {
            if (this.MetaPropertiesDockContent.IsHidden)
            {
                this.MetaPropertiesDockContent.Show(this.dockPanel);
            }
            else
            {
                this.MetaPropertiesDockContent.Hide();
            }
        }

        private void toolStripButtonOpen_Click(object sender, EventArgs e)
        {
            PickFiles();
        }
        #endregion

        #region Lazy Load Windows
        public DefaultProperties OptionsDockContent
        {
            get
            {
                if ((options != null) && options.IsDisposed) options = null;
                if (options == null) options = new DefaultProperties(this);
                return options;
            }
        }

        public TemplateBrowser TemplateBrowserDockContent
        {
            get
            {
                if ((templateBrowser != null) && templateBrowser.IsDisposed) templateBrowser = null;
                if (templateBrowser == null) templateBrowser = new TemplateBrowser(this);
                return templateBrowser;
            }
        }

        public LanguageMappings LanguageMappingsDockContent
        {
            get
            {
                if ((languageMappings != null) && languageMappings.IsDisposed) languageMappings = null;
                if (languageMappings == null) languageMappings = new LanguageMappings(this);
                return languageMappings;
            }
        }

        public DbTargetMappings DbTargetMappingsDockContent
        {
            get
            {
                if ((dbTargetMappings != null) && dbTargetMappings.IsDisposed) dbTargetMappings = null;
                if (dbTargetMappings == null) dbTargetMappings = new DbTargetMappings(this);
                return dbTargetMappings;
            }
        }

        public MetaDataBrowser MetaDataBrowserDockContent
        {
            get
            {
                if ((metaDataBrowser != null) && metaDataBrowser.IsDisposed) metaDataBrowser = null;
                if (metaDataBrowser == null) metaDataBrowser = new MetaDataBrowser(this);
                return metaDataBrowser;
            }
        }

        public UserMetaData UserMetaDataDockContent
        {
            get
            {
                if ((userMetaData != null) && userMetaData.IsDisposed) userMetaData = null;
                if (userMetaData == null) userMetaData = new UserMetaData(this);
                return userMetaData;
            }
        }

        public GlobalUserMetaData GlobalUserMetaDataDockContent
        {
            get
            {
                if ((globalUserMetaData != null) && globalUserMetaData.IsDisposed) globalUserMetaData = null;
                if (globalUserMetaData == null) globalUserMetaData = new GlobalUserMetaData(this);
                return globalUserMetaData;
            }
        }

        public MetaProperties MetaPropertiesDockContent
        {
            get
            {
                if ((metaProperties != null) && metaProperties.IsDisposed) metaProperties = null;
                if (metaProperties == null) metaProperties = new MetaProperties(this);
                return metaProperties;
            }
        }
        #endregion

        #region Replaces files that couldn't be replaced until locks were removed
        private void ProcessReplacementFiles()
        {
            DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath));
            foreach (FileInfo info in dir.GetFiles("*" + REPLACEMENT_SUFFIX))
            {
                FileInfo fileToReplace = new FileInfo(info.FullName.Replace(REPLACEMENT_SUFFIX, ".dll"));
                try
                {
                    if (fileToReplace.Exists)
                    {
                        fileToReplace.MoveTo(fileToReplace.FullName + "." + DateTime.Now.ToString("yyyyMMddhhmmss") + ".bak");
                    }

                    info.MoveTo(fileToReplace.FullName);
                }
#if DEBUG
				catch (Exception ex) { throw ex; }
#else
                catch { }
#endif
            }
        }
        #endregion

        #region Refresh the Recent files menu item
        public void RefreshRecentFiles()
        {
            // Clear the Recent Items List
            this.recentFilesToolStripMenuItem.DropDownItems.Clear();

            DefaultSettings ds = DefaultSettings.Instance;
            if (ds.RecentFiles.Count == 0)
            {
                recentFilesToolStripMenuItem.Visible = false;
            }
            else
            {
                recentFilesToolStripMenuItem.Visible = true;

                foreach (string path in ds.RecentFiles)
                {
                    ToolStripMenuItem item = new ToolStripMenuItem(path);
                    item.Click += new EventHandler(recentFilesToolStripMenuItem_Click);
                    recentFilesToolStripMenuItem.DropDownItems.Add(item);
                }
            }
        }

        public void AddRecentFile(string path)
        {

            if (settings.RecentFiles.Contains(path))
            {
                settings.RecentFiles.Remove(path);
            }

            settings.RecentFiles.Insert(0, path);
            settings.Save();

            RefreshRecentFiles();
        }

        public void RemoveRecentFile(string path)
        {
            if (settings.RecentFiles.Contains(path))
            {
                settings.RecentFiles.Remove(path);
            }
            settings.Save();

            RefreshRecentFiles();
        }
        #endregion

        #region IMyGenerationMDI Members

        public FindForm FindDialog
        {
            get { return findDialog; }
        }

        public ReplaceForm ReplaceDialog
        {
            get { return replaceDialog; }
        }

        public ScintillaConfigureDelegate ConfigureDelegate
        {
            get { return configureDelegate; }
        }

        public DockPanel DockPanel
        {
            get { return dockPanel;  }
        }

        private void docItemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = sender as ToolStripMenuItem;
            if (i != null)
            {
                IMyGenDocument mgd = this.FindDocument(i.Tag.ToString());
                mgd.DockContent.Activate();
            }
        }

        private void windowToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            windowToolStripMenuItem.DropDownItems.Clear();
            foreach (DockContent doc in this.dockPanel.Contents)
            {
                if (doc is IMyGenDocument)
                {
                    IMyGenDocument mgd = doc as IMyGenDocument;
                    if (!doc.IsHidden)
                    {

                        ToolStripMenuItem i = new ToolStripMenuItem(doc.Text, null, docItemToolStripMenuItem_Click);
                        i.Tag = mgd.DocumentIndentity;
                        if (doc.IsActivated) i.Checked = true;
                        windowToolStripMenuItem.DropDownItems.Add(i);
                    }
                }
            }
                if (windowToolStripMenuItem.DropDownItems.Count == 0) {
                }

        }

        private void dockPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        /*public SciTEProperties Properties
        {
            get { return properties; }
        }*/

        #endregion
    }
}