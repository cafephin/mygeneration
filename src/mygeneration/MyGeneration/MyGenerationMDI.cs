using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MyGeneration.Forms;
using Scintilla;
using Scintilla.Configuration.Legacy;
using Scintilla.Forms;
using WeifenLuo.WinFormsUI.Docking;
using Zeus;

namespace MyGeneration
{
    public partial class MyGenerationMDI : Form, IMyGenerationMDI
    {
        private const string DOCK_CONFIG_FILE = @"\settings\dock.config";
        private const string SCINTILLA_CONFIG_FILE = @"\settings\scintillanet.xml";
        private const string REPLACEMENT_SUFFIX = "$REPLACEMENT$.dll";

        private static ConfigFile _configFile;
        private ScintillaConfigureDelegate _scintillaConfigureDelegate;
        
        private readonly Dictionary<string, IMyGenContent> _dynamicContentWindows = new Dictionary<string, IMyGenContent>();
        
        private LanguageMappings _languageMappings;
        private DbTargetMappings _dbTargetMappings;
        private MetaDataBrowser _metaDataBrowser;
        private UserMetaData _userMetaData;
        private GlobalUserMetaData _globalUserMetaData;
        private MetaProperties _metaProperties;
        private DefaultSettingsDialog _defaultSettingsDialog;
        private TemplateBrowser _templateBrowser;
        private ConsoleForm _consoleForm;
        private ErrorsForm _errorsForm;
        private ErrorDetail _errorDetail;
        private GeneratedFilesForm _generatedFilesForm;

        private readonly string _startupPath;
        private readonly string[] _startupFiles;
        
        public MyGenerationMDI(string startupPath, params string[] args)
        {
            _startupPath = startupPath;

            // if the command line arguments contain a new location for the config file, set it.
            var argsList = new List<string>();
            string lastArg = null;
            foreach (string arg in args)
            {
                if (lastArg == "-configfile")
                {
                    string file = Zeus.FileTools.MakeAbsolute(arg, FileTools.AssemblyPath);
                    if (File.Exists(file))
                    {
                        DefaultSettings.SettingsFileName = file;
                    }
                }
                else
                {
                    argsList.Add(arg);
                }
                lastArg = arg;
            }

            // Any files that were locked when the TemplateLibrary downloaded 
            // and tried to replace them will be replaced now.
            ProcessReplacementFiles();

            InitializeComponent();

            _startupFiles = argsList.ToArray();

            EditorManager.AddNewDocumentMenuItems(newFileDynamicToolStripMenuItem_Click,
                                                  newToolStripMenuItem.DropDownItems,
                                                  toolStripDropDownButtonNew.DropDownItems);

            ContentManager.AddNewContentMenuItems(openContentDynamicToolStripMenuItem_Click, pluginsToolStripMenuItem, toolStrip1);

            PluginManager.AddHelpMenuItems(HelpMenuItem_OnClicked, helpToolStripMenuItem, 2);

            RefreshRecentFiles();
        }

        #region Loading
        private void MyGenerationMDI_OnLoad(object sender, EventArgs e)
        {
            RestoreWindowState();

            LoadScintillaConfiguration();

            LoadDockContentConfiguration();

            if (_startupFiles != null)
            {
                OpenDocuments(_startupFiles);
            }
        }

        private void LoadDockContentConfiguration()
        {
            var deserializeDockContent = new DeserializeDockContent(GetContentFromPersistString);
            var dockConfigFileName = _startupPath + DOCK_CONFIG_FILE;
            if (!File.Exists(dockConfigFileName)) return;
            
            try
            {
                MainDockPanel.LoadFromXml(dockConfigFileName, deserializeDockContent);
            }
            catch (Exception)
            {
                try
                {
                    File.Delete(dockConfigFileName);
                }
                catch
                {
                }
            }
        }

        private void LoadScintillaConfiguration()
        {
            var scintillaConfigFile = new FileInfo(_startupPath + SCINTILLA_CONFIG_FILE);
            if (scintillaConfigFile.Exists)
            {
                var maxTries = 3;
                while (maxTries > 0)
                {
                    try
                    {
                        _configFile = new ConfigurationUtility().LoadConfiguration(scintillaConfigFile.FullName) as ConfigFile;
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
                            Thread.Sleep(10);
                        }
                    }
                }
            }

            if (_configFile == null) return;

            _scintillaConfigureDelegate = _configFile.MasterScintilla.Configure;
            ZeusScintillaControl.StaticConfigure = _scintillaConfigureDelegate;
        }

        private void RestoreWindowState()
        {
            switch (DefaultSettings.Instance.WindowState)
            {
                case "Maximized":
                    WindowState = FormWindowState.Maximized;
                    break;
                case "Minimized":
                    WindowState = FormWindowState.Minimized;
                    break;
                case "Normal":
                    var x = Convert.ToInt32(DefaultSettings.Instance.WindowPosLeft);
                    var y = Convert.ToInt32(DefaultSettings.Instance.WindowPosTop);
                    var w = Convert.ToInt32(DefaultSettings.Instance.WindowPosWidth);
                    var h = Convert.ToInt32(DefaultSettings.Instance.WindowPosHeight);
                    Location = new Point(x, y);
                    Size = new Size(w, h);
                    break;
            }
        }
        #endregion

        #region Closing
        private void MyGenerationMDI_FormClosing(object sender, FormClosingEventArgs e)
        {
            var allowPrevent = true; 
            var allowSave = true;
            if (e.CloseReason == CloseReason.TaskManagerClosing)
            {
                allowSave = false;
            }
            else if (e.CloseReason == CloseReason.WindowsShutDown ||
                e.CloseReason == CloseReason.ApplicationExitCall)
            {
                allowPrevent = false;
            }

            if (!ZeusProcessManager.IsDormant)
            {
                var dialogResult = MessageBox.Show(this,
                                                   "There are templates currently being executed. Would you like to kill them?",
                                                   "Warning",
                                                   MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    ZeusProcessManager.KillAll();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            if (allowSave && !Shutdown(allowPrevent))
            {
                e.Cancel = true;
            }
            else
            {
                SaveWindowState();
            }
        }

        private void SaveWindowState()
        {
            try
            {
                switch (WindowState)
                {
                    case FormWindowState.Maximized:
                        DefaultSettings.Instance.WindowState = "Maximized";
                        break;
                    case FormWindowState.Minimized:
                        DefaultSettings.Instance.WindowState = "Minimized";
                        break;
                    case FormWindowState.Normal:
                        DefaultSettings.Instance.WindowState = "Normal";
                        DefaultSettings.Instance.WindowPosLeft = Location.X.ToString();
                        DefaultSettings.Instance.WindowPosTop = Location.Y.ToString();
                        DefaultSettings.Instance.WindowPosWidth = Size.Width.ToString();
                        DefaultSettings.Instance.WindowPosHeight = Size.Height.ToString();
                        break;
                }
                DefaultSettings.Instance.Save();
            }
            catch
            {
            }
        }
        #endregion

        private void PickFiles()
        {
            var openFileDialog = new OpenFileDialog
                                 {
                                     InitialDirectory = Directory.GetCurrentDirectory(),
                                     Filter = EditorManager.OpenFileDialogString,
                                     RestoreDirectory = true,
                                     Multiselect = true
                                 };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                OpenDocuments(openFileDialog.FileNames);
            }
        }

        private void OpenContent(params string[] keys)
        {
            foreach (var key in keys)
            {
                if (_dynamicContentWindows.ContainsKey(key))
                {
                    IMyGenContent mygenContent = _dynamicContentWindows[key];
                    if (mygenContent.DockContent.Visible)
                    {
                        mygenContent.DockContent.Hide();
                    }
                    else
                    {
                        mygenContent.DockContent.Show(MainDockPanel);
                    }
                }
                else
                {
                    IMyGenContent mygenContent = ContentManager.CreateContent(this, key);
                    if (mygenContent != null)
                    {
                        _dynamicContentWindows[key] = mygenContent;
                        mygenContent.DockContent.Show(MainDockPanel);
                    }
                }
            }
        }

        private void ExecuteSimplePlugin(params string[] keys)
        {
            try
            {
                foreach (string key in keys)
                {
                    if (PluginManager.SimplePluginManagers.ContainsKey(key))
                    {
                        PluginManager.SimplePluginManagers[key].Execute(this);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorsOccurred(ex);
            }
        }

        public void CreateDocument(params string[] fileTypes)
        {
            foreach (var fileType in fileTypes)
            {
                IMyGenDocument mygenDoc = EditorManager.CreateDocument(this, fileType);
                if (mygenDoc != null)
                    mygenDoc.DockContent.Show(MainDockPanel);
            }
        }

        public void OpenDocuments(params string[] filenames)
        {
            foreach (var file in filenames)
            {
                var info = new FileInfo(file);

                if (!info.Exists) continue;

                if (IsDocumentOpen(info.FullName))
                {
                    FindDocument(info.FullName).DockContent.Activate();
                }
                else
                {
                    var isOpened = false;
                    IMyGenDocument mygenDoc = EditorManager.OpenDocument(this, info.FullName);
                    if (mygenDoc != null)
                    {
                        isOpened = true;
                        mygenDoc.DockContent.Show(MainDockPanel);
                        AddRecentFile(info.FullName);
                    }

                    if (!isOpened)
                    {
                        MessageBox.Show(this, info.Name + ": unknown file type", "Unknown file type");
                    }
                }
            }
        }

        private IDockContent GetContentFromPersistString(string persistString)
        {
            IDockContent content = null;
            string[] parsedStrings = persistString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            if (parsedStrings.Length == 1)
            {
                string type = parsedStrings[0];
                if (type == typeof(LanguageMappings).ToString())
                {
                    content = LanguageMappingsDockContent;
                }
                else if (type == typeof(DbTargetMappings).ToString())
                {
                    content = DbTargetMappingsDockContent;
                }
                else if (type == typeof(MetaProperties).ToString())
                {
                    content = MetaPropertiesDockContent;
                }
                else if (type == typeof(MetaDataBrowser).ToString())
                {
                    content = MetaDataBrowserDockContent;
                }
                else if (type == typeof(UserMetaData).ToString())
                {
                    content = UserMetaDataDockContent;
                }
                else if (type == typeof(GlobalUserMetaData).ToString())
                {
                    content = GlobalUserMetaDataDockContent;
                }
                else if (type == typeof(TemplateBrowser).ToString())
                {
                    content = TemplateBrowserDockContent;
                }
                else if (type == typeof(DefaultSettingsDialog).ToString() && DefaultSettings.Instance.EnableDocumentStyleSettings)
                {
                    content = DefaultSettingsDialog;
                }
                else if (type == typeof(ErrorsForm).ToString())
                {
                    content = ErrorsDockContent;
                }
                else if (type == typeof(GeneratedFilesForm).ToString())
                {
                    content = GeneratedFilesDockContent;
                }
                else if (type == typeof(ConsoleForm).ToString())
                {
                    content = ConsoleDockContent;
                }
                else
                {
                    // Preload all dynamicContentWindows here if needed
                    foreach (IContentManager cm in PluginManager.ContentManagers.Values)
                    {
                        _dynamicContentWindows[cm.Name] = cm.Create(this);
                    }
                    foreach (IMyGenContent c in _dynamicContentWindows.Values)
                    {
                        if (type == c.GetType().ToString())
                        {
                            content = c.DockContent;
                            break;
                        }
                    }
                }
            }
            else if (parsedStrings.Length >= 2)
            {
                string type = parsedStrings[0];
                string arg = parsedStrings[1];

                IMyGenDocument doc;
                if (string.Equals(type, "file", StringComparison.CurrentCultureIgnoreCase))
                {
                    doc = EditorManager.OpenDocument(this, arg);
                    if (doc != null) content = doc.DockContent;
                }
                if (string.Equals(type, "type", StringComparison.CurrentCultureIgnoreCase))
                {
                    doc = EditorManager.CreateDocument(this, arg);
                    if (doc != null) content = doc.DockContent;
                }
            }

            return content;
        }

        private bool Shutdown(bool allowPrevent)
        {
            var shutdown = true;
            try
            {
                string dockConfigFileName = _startupPath + DOCK_CONFIG_FILE;

                IMyGenContent mygenContent = null;
                DockContentCollection dockContentCollection = MainDockPanel.Contents;
                var canClose = true;

                if (allowPrevent && !ZeusProcessManager.IsDormant)
                {
                    return false;
                }

                foreach (IDockContent dockContent in dockContentCollection)
                {
                    mygenContent = dockContent as IMyGenContent;

                    // We need the MetaDataBrowser window to be closed last 
                    // because it houses the UserMetaData
                    if (!(mygenContent is MetaDataBrowser))
                    {
                        canClose = mygenContent.CanClose(allowPrevent);

                        if (allowPrevent && !canClose)
                        {
                            shutdown = false;
                            break;
                        }
                    }

                    if (dockContent.DockHandler.IsHidden)
                    {
                        dockContent.DockHandler.Close();
                    }
                }

                if (shutdown)
                {
                    MainDockPanel.SaveAsXml(dockConfigFileName);
                }
            }
            catch
            {
                shutdown = true;
            }

            return shutdown;
        }

        #region Drag n' Drop
        private void MyGenerationMDI_DragDrop(object sender, DragEventArgs e)
        {
            string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
            OpenDocuments(filenames);
        }

        private void MyGenerationMDI_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                bool foundValidFile = false;
                string[] filenames = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (string filename in filenames)
                {
                    int idx = filename.LastIndexOf('.');
                    if (idx >= 0) {
                        string ext = filename.Substring(idx + 1);
                    foreach (IEditorManager em in PluginManager.EditorManagers.Values)
                    {
                        if (em.FileExtensions.ContainsKey(ext))
                        {
                            foundValidFile = true;
                            break;
                        }
                    }
                    }
                    if (foundValidFile) break;
                }

                // allow them to continue
                // (without this, the cursor stays a "NO" symbol
                if (foundValidFile)
                {
                    e.Effect = DragDropEffects.All;
                }
            }
        }
        #endregion

        #region DockManager Helper Methods
        public bool IsDocumentOpen(string text, params IMyGenDocument[] docsToExclude)
        {
            IMyGenDocument doc = FindDocument(text, docsToExclude);
            return (doc != null);
        }

        public IMyGenDocument FindDocument(string text, params IMyGenDocument[] docsToExclude)
        {
            IMyGenDocument found = null;
            if (MainDockPanel.DocumentStyle == DocumentStyle.SystemMdi)
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
                foreach (IDockContent content in MainDockPanel.Documents)
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

        private void dockPanel_ActiveContentChanged(object sender, EventArgs e)
        {
            IDockContent activeContent = MainDockPanel.ActiveContent;
            if (activeContent is IEditControl)
            {
                ToolStripManager.RevertMerge(toolStrip1);
            }
            else if (activeContent is IMyGenDocument)
            {
                ToolStripManager.RevertMerge(toolStrip1);
                var mgd = activeContent as IMyGenDocument;
                if (mgd.ToolStrip != null)
                {
                    ToolStripManager.Merge(mgd.ToolStrip, toolStrip1);
                }
            }
            else if (activeContent == null)
            {
                var foundDoc = MainDockPanel.Contents.Cast<DockContent>().Any(c => c is IMyGenDocument && !c.IsHidden);
                if (!foundDoc) ToolStripManager.RevertMerge(toolStrip1);
            }
        }

        private int _indexImgAnimate = -1;

        private void timerImgAnimate_Tick(object sender, EventArgs e)
        {
            _indexImgAnimate = _indexImgAnimate >= 3 ? 0 : _indexImgAnimate + 1;
            switch (_indexImgAnimate)
            {
                case 0:
                    toolStripStatusQueue.Image = Zeus.SharedResources.Refresh16x16_1;
                    break;
                case 1:
                    toolStripStatusQueue.Image = Zeus.SharedResources.Refresh16x16_2;
                    break;
                case 2:
                    toolStripStatusQueue.Image = Zeus.SharedResources.Refresh16x16_3;
                    break;
                case 3:
                    toolStripStatusQueue.Image = Zeus.SharedResources.Refresh16x16_4;
                    break;
            }
            toolStripStatusQueue.Invalidate();
        }

        #region Menu Event Handlers
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PickFiles();
        }

        private void newFileDynamicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem i = sender as ToolStripMenuItem;
            CreateDocument(i.Text);
        }

        private void openContentDynamicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var toolStripItem = sender as ToolStripItem;
            if ((Type) toolStripItem.Tag == typeof(IContentManager))
            {
                OpenContent(string.IsNullOrEmpty(toolStripItem.Text) ? toolStripItem.ToolTipText : toolStripItem.Text);
            }
            else
            {
                ExecuteSimplePlugin(string.IsNullOrEmpty(toolStripItem.Text) ? toolStripItem.ToolTipText : toolStripItem.Text);
            }
        }

        private void ExitMenuItem_OnClicked(object sender, EventArgs e)
        {
            Close();
        }

        private void AboutMenuItem_OnClicked(object sender, EventArgs e)
        {
            new AboutBox().ShowDialog(this);
        }

        private void SettingsMenuItem_OnClicked(object sender, EventArgs e)
        {
            if (!DefaultSettings.Instance.EnableDocumentStyleSettings)
            {
                if (_defaultSettingsDialog != null)
                {
                    DefaultSettingsDialog.Hide();
                    _defaultSettingsDialog = null;
                }
                var defaultProperties = new DefaultSettingsDialog(this);
                defaultProperties.ShowDialog(this);
            }
            else
            {
                if (DefaultSettingsDialog.IsHidden)
                {
                    DefaultSettingsDialog.Show(MainDockPanel);
                }
                else
                {
                    DefaultSettingsDialog.Activate();
                }
            }
        }

        private void HelpMenuItem_OnClicked(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;
            if (item != null)
            {
                Zeus.WindowsTools.LaunchHelpFile(_startupPath + item.Tag, ProcessWindowStyle.Maximized, createNoWindow: true);
            }
        }

        private void RecentFileMenuItem_OnClicked(object sender, EventArgs e)
        {
            if (!(sender is ToolStripMenuItem)) return;
            
            var item = sender as ToolStripMenuItem;
            var file = item.Text;
            if (File.Exists(file))
            {
                AddRecentFile(item.Text);
                OpenDocuments(item.Text);
            }
            else
            {
                MessageBox.Show(this, item.Text + " no longer exists.", "File Missing");
                RemoveRecentFile(item.Text);
            }
        }

        private void CheckForUpdateMenuItem_OnClicked(object sender, EventArgs e)
        {
            new ApplicationReleases().ShowDialog(this);
        }

        private void DocMenuItem_OnClicked(object sender, EventArgs e)
        {
            var i = sender as ToolStripMenuItem;
            if (i != null)
            {
                IMyGenDocument mgd = FindDocument(i.Tag.ToString());
                mgd.DockContent.Activate();
            }
        }

        private void WindowMenu_OnDropDownOpening(object sender, EventArgs e)
        {
            windowToolStripMenuItem.DropDownItems.Clear();
            foreach (var dockContent in MainDockPanel.Contents)
            {
                var doc = (DockContent) dockContent;
                if (!(doc is IMyGenDocument)) continue;
                
                var mgd = doc as IMyGenDocument;
                if (doc.IsHidden) continue;

                var toolStripMenuItem = new ToolStripMenuItem(doc.Text, null, DocMenuItem_OnClicked) {Tag = mgd.DocumentIndentity};
                if (doc.IsActivated) toolStripMenuItem.Checked = true;
                windowToolStripMenuItem.DropDownItems.Add(toolStripMenuItem);
            }
        }
        #endregion

        #region Toolstrip Button Event Handlers
        private void ToolStripOpenButton_OnClicked(object sender, EventArgs e)
        {
            PickFiles();
        }

        private void ToolStripTemplateBrowserButton_OnClicked(object sender, EventArgs e)
        {
            if (TemplateBrowserDockContent.IsHidden)
            {
                TemplateBrowserDockContent.Show(MainDockPanel);
            }
            else
            {
                TemplateBrowserDockContent.Activate();
            }
        }

        private void ToolStripSettingsButton_OnClicked(object sender, EventArgs e)
        {
            if (!DefaultSettings.Instance.EnableDocumentStyleSettings)
            {
                if (_defaultSettingsDialog != null)
                {
                    DefaultSettingsDialog.Hide();
                    _defaultSettingsDialog = null;
                }
                var dsd = new DefaultSettingsDialog(this);
                dsd.ShowDialog(this);
            }
            else
            {
                if (DefaultSettingsDialog.IsHidden)
                {
                    DefaultSettingsDialog.Show(MainDockPanel);
                }
                else
                {
                    DefaultSettingsDialog.Hide();
                }
            }
        }

        private void ToolStripMetaBrowserButton_OnClicked(object sender, EventArgs e)
        {
            if (MetaDataBrowserDockContent.IsHidden)
            {
                MetaDataBrowserDockContent.Show(MainDockPanel);
            }
            else
            {
                MetaDataBrowserDockContent.Activate();
            }
        }

        private void ToolStripMyMetaPropertiesButton_OnClicked(object sender, EventArgs e)
        {
            if (MetaPropertiesDockContent.IsHidden)
            {
                MetaPropertiesDockContent.Show(MainDockPanel);
            }
            else
            {
                MetaPropertiesDockContent.Activate();
            }
        }

        private void ToolStripLanguageMappingsButton_OnClicked(object sender, EventArgs e)
        {
            if (LanguageMappingsDockContent.IsHidden)
            {
                LanguageMappingsDockContent.Show(MainDockPanel);
            }
            else
            {
                LanguageMappingsDockContent.Activate();
            }
        }

        private void ToolStripDbTargetMappingsButton_OnClicked(object sender, EventArgs e)
        {
            if (DbTargetMappingsDockContent.IsHidden)
            {
                DbTargetMappingsDockContent.Show(MainDockPanel);
            }
            else
            {
                DbTargetMappingsDockContent.Activate();
            }
        }

        private void ToolStripLocalAliasesButton_OnClicked(object sender, EventArgs e)
        {
            if (UserMetaDataDockContent.IsHidden)
            {
                UserMetaDataDockContent.Show(MainDockPanel);
            }
            else
            {
                UserMetaDataDockContent.Activate();
            }
        }

        private void ToolStripGlobalAliasesButton_OnClicked(object sender, EventArgs e)
        {
            if (GlobalUserMetaDataDockContent.IsHidden)
            {
                GlobalUserMetaDataDockContent.Show(MainDockPanel);
            }
            else
            {
                GlobalUserMetaDataDockContent.Activate();
            }
        }

        private void ToolStripConsoleButton_OnClicked(object sender, EventArgs e)
        {
            if (ConsoleDockContent.IsHidden)
            {
                ConsoleDockContent.Show(MainDockPanel);
            }
            else
            {
                ConsoleDockContent.Activate();
            }
        }

        private void ToolStripErrorButton_OnClicked(object sender, EventArgs e)
        {
            if (ErrorsDockContent.IsHidden)
            {
                ErrorsDockContent.Show(MainDockPanel);
            }
            else
            {
                ErrorsDockContent.Activate();
            }
        }

        private void ToolStripRecentlyGeneratedFilesButton_OnClicked(object sender, EventArgs e)
        {
            if (GeneratedFilesDockContent.IsHidden)
            {
                GeneratedFilesDockContent.Show(MainDockPanel);
            }
            else
            {
                GeneratedFilesDockContent.Activate();
            }
        }

        private void ToolStripOpenGeneratedOutputFolderButton_OnClicked(object sender, EventArgs e)
        {
            Process p = new Process();
            p.StartInfo.FileName = "explorer";
            p.StartInfo.Arguments = "/e," + DefaultSettings.Instance.DefaultOutputDirectory;
            p.StartInfo.UseShellExecute = true;
            p.Start();
        }
        #endregion

        #region Lazy Load Windows
        public GeneratedFilesForm GeneratedFilesDockContent
        {
            get
            {
                if ((_generatedFilesForm != null) && _generatedFilesForm.IsDisposed) _generatedFilesForm = null;
                if (_generatedFilesForm == null) _generatedFilesForm = new GeneratedFilesForm(this);
                return _generatedFilesForm;
            }
        }

        public ConsoleForm ConsoleDockContent
        {
            get
            {
                if ((_consoleForm != null) && _consoleForm.IsDisposed) _consoleForm = null;
                if (_consoleForm == null) _consoleForm = new ConsoleForm(this);
                return _consoleForm;
            }
        }

        public ErrorsForm ErrorsDockContent
        {
            get
            {
                if ((_errorsForm != null) && _errorsForm.IsDisposed) _errorsForm = null;
                if (_errorsForm == null) _errorsForm = new ErrorsForm(this);
                return _errorsForm;
            }
        }

        public ErrorDetail ErrorDetailDockContent
        {
            get
            {
                if ((_errorDetail != null) && _errorDetail.IsDisposed) _errorDetail = null;
                if (_errorDetail == null) _errorDetail = new ErrorDetail(this);
                return _errorDetail;
            }
        }

        public DefaultSettingsDialog DefaultSettingsDialog
        {
            get
            {
                if (_defaultSettingsDialog != null && _defaultSettingsDialog.IsDisposed) _defaultSettingsDialog = null;
                return _defaultSettingsDialog ?? (_defaultSettingsDialog = new DefaultSettingsDialog(this));
            }
        }

        public TemplateBrowser TemplateBrowserDockContent
        {
            get
            {
                if ((_templateBrowser != null) && _templateBrowser.IsDisposed) _templateBrowser = null;
                if (_templateBrowser == null) _templateBrowser = new TemplateBrowser(this);
                return _templateBrowser;
            }
        }

        public LanguageMappings LanguageMappingsDockContent
        {
            get
            {
                if ((_languageMappings != null) && _languageMappings.IsDisposed) _languageMappings = null;
                if (_languageMappings == null) _languageMappings = new LanguageMappings(this);
                return _languageMappings;
            }
        }

        public DbTargetMappings DbTargetMappingsDockContent
        {
            get
            {
                if ((_dbTargetMappings != null) && _dbTargetMappings.IsDisposed) _dbTargetMappings = null;
                if (_dbTargetMappings == null) _dbTargetMappings = new DbTargetMappings(this);
                return _dbTargetMappings;
            }
        }

        public MetaDataBrowser MetaDataBrowserDockContent
        {
            get
            {
                if ((_metaDataBrowser != null) && _metaDataBrowser.IsDisposed) _metaDataBrowser = null;
                if (_metaDataBrowser == null) _metaDataBrowser = new MetaDataBrowser(this,
                    MetaPropertiesDockContent, UserMetaDataDockContent, GlobalUserMetaDataDockContent);
                return _metaDataBrowser;
            }
        }

        public UserMetaData UserMetaDataDockContent
        {
            get
            {
                if ((_userMetaData != null) && _userMetaData.IsDisposed) _userMetaData = null;
                if (_userMetaData == null)
                {
                    _userMetaData = new UserMetaData(this);
                    _userMetaData.MetaDataBrowser = MetaDataBrowserDockContent;
                }
                return _userMetaData;
            }
        }

        public GlobalUserMetaData GlobalUserMetaDataDockContent
        {
            get
            {
                if ((_globalUserMetaData != null) && _globalUserMetaData.IsDisposed) _globalUserMetaData = null;
                if (_globalUserMetaData == null)
                {
                    _globalUserMetaData = new GlobalUserMetaData(this);
                    _globalUserMetaData.MetaDataBrowser = MetaDataBrowserDockContent;
                }
                return _globalUserMetaData;
            }
        }

        public MetaProperties MetaPropertiesDockContent
        {
            get
            {
                if ((_metaProperties != null) && _metaProperties.IsDisposed) _metaProperties = null;
                if (_metaProperties == null) _metaProperties = new MetaProperties(this);
                return _metaProperties;
            }
        }
        #endregion

        private void ProcessReplacementFiles()
        {
            var dir = new DirectoryInfo(Path.GetDirectoryName(Application.ExecutablePath));
            foreach (FileInfo info in dir.GetFiles("*" + REPLACEMENT_SUFFIX))
            {
                var fileToReplace = new FileInfo(info.FullName.Replace(REPLACEMENT_SUFFIX, ".dll"));
                try
                {
                    if (fileToReplace.Exists)
                    {
                        fileToReplace.MoveTo(fileToReplace.FullName + "." + DateTime.Now.ToString("yyyyMMddhhmmss") + ".bak");
                    }

                    info.MoveTo(fileToReplace.FullName);
                }
                catch { }
            }
        }
        
        #region Refresh Recent Files
        private void RefreshRecentFiles()
        {
            recentFilesToolStripMenuItem.DropDownItems.Clear();

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
                    var item = new ToolStripMenuItem(path);
                    item.Click += RecentFileMenuItem_OnClicked;
                    recentFilesToolStripMenuItem.DropDownItems.Add(item);
                }
            }
        }

        private void AddRecentFile(string path)
        {

            if (DefaultSettings.Instance.RecentFiles.Contains(path))
            {
                DefaultSettings.Instance.RecentFiles.Remove(path);
            }

            DefaultSettings.Instance.RecentFiles.Insert(0, path);
            DefaultSettings.Instance.Save();

            RefreshRecentFiles();
        }

        private void RemoveRecentFile(string path)
        {
            if (DefaultSettings.Instance.RecentFiles.Contains(path))
            {
                DefaultSettings.Instance.RecentFiles.Remove(path);
            }
            DefaultSettings.Instance.Save();

            RefreshRecentFiles();
        }
        #endregion

        #region Show OLEDBDialog Dialog

        protected string BrowseOleDbConnectionString(string connstr)
        {
            var dl = new MSDASC.DataLinksClass {hWnd = Handle.ToInt32()};

            var conn = new ADODB.Connection {ConnectionString = connstr};
            object objCn = conn;

            return dl.PromptEdit(ref objCn) ? conn.ConnectionString : null;
        }
        #endregion

        #region IMyGenerationMDI Members

        public IZeusController ZeusController
        {
            get { return Zeus.ZeusController.Instance; }
        }

        private readonly FindForm _findForm = new FindForm();
        public FindForm FindDialog
        {
            get { return _findForm; }
        }

        private readonly ReplaceForm _replaceForm = new ReplaceForm();
        public ReplaceForm ReplaceDialog
        {
            get { return _replaceForm; }
        }

        public ScintillaConfigureDelegate ConfigureDelegate
        {
            get { return _scintillaConfigureDelegate; }
        }

        public DockPanel DockPanel
        {
            get { return MainDockPanel; }
        }

        public void SendAlert(IMyGenContent sender, string command, params object[] args)
        {
            IMyGenContent contentItem = null;
            DockContentCollection contents = MainDockPanel.Contents;

            DefaultSettings settings = DefaultSettings.Instance;

            for (int i = 0; i < contents.Count; i++)
            {
                contentItem = contents[i] as IMyGenContent;

                if (contentItem != null)
                {
                    contentItem.ProcessAlert(sender, command, args);
                }
            }
        }

        public object PerformMdiFunction(IMyGenContent sender, string function, params object[] args)
        {
            if (function.Equals("getstaticdbroot", StringComparison.CurrentCultureIgnoreCase))
            {
                return MetaDataBrowser.StaticMyMetaObj;
            }
            
            if (function.Equals("showoledbdialog", StringComparison.CurrentCultureIgnoreCase) &&
                args.Length == 1)
            {
                return BrowseOleDbConnectionString(args[0].ToString());
            }
            
            if (function.Equals("executionqueuestart", StringComparison.CurrentCultureIgnoreCase))
            {
                toolStripStatusQueue.Visible = true;
                timerImgAnimate.Start();
            }
            else if (function.Equals("executionqueueupdate", StringComparison.CurrentCultureIgnoreCase))
            {
                if (ZeusProcessManager.ProcessCount == 0)
                {
                    timerImgAnimate.Stop();
                    toolStripStatusQueue.Visible = false;
                }
                else if (ZeusProcessManager.ProcessCount > 0)
                {
                    toolStripStatusQueue.Visible = true;
                    timerImgAnimate.Start();
                }
            }
            else if (function.Equals("showerrordetail", StringComparison.CurrentCultureIgnoreCase) &&
                args.Length >= 1)
            {
                if (args[0] is List<IMyGenError>)
                {
                    List<IMyGenError> errors = args[0] as List<IMyGenError>;
                    ErrorDetailDockContent.Update(errors[0]);
                    if (ErrorDetailDockContent.IsHidden)
                    {
                        ErrorDetailDockContent.Show(MainDockPanel);
                    }
                    else
                    {
                        ErrorDetailDockContent.Activate();
                    }
                }
            }
            else if (function.Equals("navigatetotemplateerror", StringComparison.CurrentCultureIgnoreCase) &&
                args.Length >= 1)
            {
                if (args[0] is IMyGenError)
                {
                    IMyGenError error = args[0] as IMyGenError;
                    TemplateEditor edit = null;

                    if (string.IsNullOrEmpty(error.SourceFile))
                    {
                        //it's a new unsaved template
                        bool isopen = IsDocumentOpen(error.TemplateIdentifier);
                        if (isopen)
                        {
                            edit = FindDocument(error.TemplateIdentifier) as TemplateEditor;
                            edit.Activate();
                        }
                    }
                    else
                    {
                        FileInfo file = new FileInfo(error.TemplateFileName);
                        if (file.Exists)
                        {
                            bool isopen = IsDocumentOpen(file.FullName);

                            if (!isopen)
                            {
                                edit = new TemplateEditor(this);
                                edit.FileOpen(file.FullName);
                            }
                            else
                            {
                                edit = FindDocument(file.FullName) as TemplateEditor;
                                if (edit != null)
                                {
                                    edit.Activate();
                                }
                            }
                        }
                    }

                    if (edit != null)
                    {
                        edit.NavigateTo(error);
                    }
                }
            }

            else if (function.Equals("getmymetadbdriver", StringComparison.CurrentCultureIgnoreCase))
            {
                return DefaultSettings.Instance.DbDriver;
            }
            else if (function.Equals("getmymetaconnection", StringComparison.CurrentCultureIgnoreCase))
            {
                return DefaultSettings.Instance.ConnectionString;
            }
            else if (function.Equals("openfile", StringComparison.CurrentCultureIgnoreCase) &&
                args.Length == 1)
            {
                if (args[0] is List<FileInfo>)
                {
                    List<FileInfo> files = args[0] as List<FileInfo>;
                    foreach (FileInfo fi in files)
                    {
                        Zeus.WindowsTools.LaunchFile(fi.FullName);
                    }
                }
                else if (args[0] is FileInfo)
                {
                    FileInfo file = args[0] as FileInfo;
                    Zeus.WindowsTools.LaunchFile(file.FullName);
                }
                else if (args[0] is String)
                {
                    Zeus.WindowsTools.LaunchFile(args[0].ToString());
                }
            }
            return null;
        }

        public IMyGenConsole Console
        {
            get { return ConsoleDockContent; }
        }

        public IMyGenErrorList ErrorList
        {
            get { return ErrorsDockContent; }
        }

        public void WriteConsole(string text, params object[] args)
        {
            ConsoleDockContent.Write(text, args);
        }

        public void ErrorsOccurred(params Exception[] exs)
        {
            ErrorsDockContent.AddErrors(exs);
            foreach (Exception ex in exs) ConsoleDockContent.Write(ex);
        }

        #endregion

        #region Error Handling

        public void UnhandledExceptions(object sender, UnhandledExceptionEventArgs args)
        {
            try
            {
                // Most likey the application is terminating on this method
                Exception ex = (Exception)args.ExceptionObject;
                HandleError(ex);
            }
            catch { }
        }

        public void OnThreadException(object sender, ThreadExceptionEventArgs t)
        {
            try
            {
                Exception ex = t.Exception;
                HandleError(ex);
            }
            catch { }
        }

        private void HandleError(Exception ex)
        {
            try
            {
                if (ErrorsDockContent != null)
                {
                    ErrorsDockContent.AddErrors(ex);
                }
                if (ConsoleDockContent != null)
                {
                    ConsoleDockContent.Write(ex);
                }
            }
            catch { }
        }
        #endregion
    }
}