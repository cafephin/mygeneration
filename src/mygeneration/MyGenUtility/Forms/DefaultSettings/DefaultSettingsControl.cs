using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using MyMeta;
using WeifenLuo.WinFormsUI;
using WeifenLuo.WinFormsUI.Docking;

namespace MyGeneration
{
    public partial class DefaultSettingsControl : UserControl
    {
        private string _lastLoadedConnection = string.Empty;
        private dbRoot _myMeta;
        private Color _defaultOleDbButtonColor;
        private DataTable _driversTable;
        private IMyGenerationMDI _mdi;

        public ShowOleDbDialogHandler ShowOleDbDialog;
        public delegate void AfterSaveDelegate();
        public event EventHandler AfterSave;

        public DefaultSettingsControl()
        {
            InitializeComponent();
        }

        public void Initialize(IMyGenerationMDI mdi)
        {
            _mdi = mdi;
        }

        public void Populate()
        {
            _myMeta = new dbRoot();
            _defaultOleDbButtonColor = OleDbButton.BackColor;

            DbDriverComboBox.DisplayMember = "DISPLAY";
            DbDriverComboBox.ValueMember = "VALUE";
            DbDriverComboBox.DataSource = DriversTable;

            switch (DefaultSettings.Instance.DbDriver)
            {
                case "PERVASIVE":
                case "POSTGRESQL":
                case "POSTGRESQL8":
                case "FIREBIRD":
                case "INTERBASE":
                case "SQLITE":
                case "MYSQL2":
#if !IGNORE_VISTA
                case "VISTADB":
#endif
                case "ISERIES":
                case "NONE":
                case "":
                    OleDbButton.Enabled = false;
                    break;
            }

            ConnectionStringTextBox.Enabled = true;

            DbDriverComboBox.SelectedValue = DefaultSettings.Instance.DbDriver;
            ConnectionStringTextBox.Text = DefaultSettings.Instance.ConnectionString;
            LanguageFileTextBox.Text = DefaultSettings.Instance.LanguageMappingFile;
            DbTargetFileTextBox.Text = DefaultSettings.Instance.DbTargetMappingFile;
            UserMetaDataFileTextBox.Text = DefaultSettings.Instance.UserMetaDataFileName;

            _myMeta.ShowDefaultDatabaseOnly = DefaultSettings.Instance.ShowDefaultDatabaseOnly;
            _myMeta.LanguageMappingFileName = DefaultSettings.Instance.LanguageMappingFile;
            _myMeta.DbTargetMappingFileName = DefaultSettings.Instance.DbTargetMappingFile;

            LanguageComboBox.Enabled = true;
            TargetDbComboBox.Enabled = true;

            PopulateLanguages();
            PopulateDbTargets();

            LanguageComboBox.SelectedItem = DefaultSettings.Instance.Language;
            TargetDbComboBox.SelectedItem = DefaultSettings.Instance.DbTarget;

            DbUserMetaMappingsTextBox.Text = DefaultSettings.Instance.DatabaseUserDataXmlMappingsString;

            CopyOutputToClipboardCheckBox.Checked = DefaultSettings.Instance.EnableClipboard;
            RunTemplatesAsyncCheckBox.Checked = DefaultSettings.Instance.ExecuteFromTemplateBrowserAsync;
            ShowDefaultDbOnlyCheckBox.Checked = DefaultSettings.Instance.ShowDefaultDatabaseOnly;
            ShowConsoleOutputCheckBox.Checked = DefaultSettings.Instance.ConsoleWriteGeneratedDetails;
            DocumentStyleSettingsCheckBox.Checked = DefaultSettings.Instance.EnableDocumentStyleSettings;
            ShowLineNumbersCheckBox.Checked = DefaultSettings.Instance.EnableLineNumbering;
            TabSizeTextBox.Text = DefaultSettings.Instance.Tabs.ToString();
            DefaultTemplatePathTextBox.Text = DefaultSettings.Instance.DefaultTemplateDirectory;
            OutputPathTextBox.Text = DefaultSettings.Instance.DefaultOutputDirectory;

            CheckForUpdatesCheckBox.Checked = DefaultSettings.Instance.CheckForNewBuild;
            DomainOverrideCheckBox.Checked = DefaultSettings.Instance.DomainOverride;

            var timeout = DefaultSettings.Instance.ScriptTimeout;
            if (timeout == -1)
            {
                checkBoxDisableTimeout.Checked = true;
                checkBoxDisableTimeout_CheckedChanged(this, new EventArgs());
            }
            else
            {
                checkBoxDisableTimeout.Checked = false;
                checkBoxDisableTimeout_CheckedChanged(this, new EventArgs());
                textBoxTimeout.Text = timeout.ToString();
            }

            UseProxyServerCheckBox.Checked = DefaultSettings.Instance.UseProxyServer;
            ProxyServerTextBox.Text = DefaultSettings.Instance.ProxyServerUri;
            ProxyUserTextBox.Text = DefaultSettings.Instance.ProxyAuthUsername;
            ProxyPasswordTextBox.Text = DefaultSettings.Instance.ProxyAuthPassword;
            ProxyDomainTextBox.Text = DefaultSettings.Instance.ProxyAuthDomain;

            ReloadSavedDbConnections();

            var encodings = Encoding.GetEncodings();
            CodePageComboBox.Items.Add(string.Empty);
            foreach (EncodingInfo encoding in encodings)
            {
                var windowsCodePage = encoding.CodePage.ToString() + ": " + encoding.DisplayName;
                var idx = CodePageComboBox.Items.Add(windowsCodePage);

                if (encoding.CodePage == DefaultSettings.Instance.CodePage)
                {
                    CodePageComboBox.SelectedIndex = idx;
                }
            }

            FontTextBox.Text = DefaultSettings.Instance.FontFamily;
        }

        public bool Save()
        {
            BindControlsToSettings();
            DefaultSettings.Instance.Save();
            return true;
        }

        protected void OnAfterSave()
        {
            if (AfterSave != null) AfterSave(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            DefaultSettings.Instance.DiscardChanges();
        }

        public bool ConnectionInfoModified
        {
            get
            {
                var info = DefaultSettings.Instance.SavedConnections[_lastLoadedConnection] as ConnectionInfo;

                if ((_lastLoadedConnection == string.Empty)
                    || (DefaultSettings.Instance.SavedConnections.ContainsKey(_lastLoadedConnection)))
                {
                    return false;
                }

                return (info.Driver != DbDriverComboBox.SelectedValue.ToString()) ||
                       (info.ConnectionString != ConnectionStringTextBox.Text) ||
                       (info.LanguagePath != LanguageFileTextBox.Text) ||
                       (info.Language != LanguageComboBox.Text) ||
                       (info.DbTargetPath != DbTargetFileTextBox.Text) ||
                       (info.DbTarget != TargetDbComboBox.Text) ||
                       (info.UserMetaDataPath != UserMetaDataFileTextBox.Text) ||
                       (info.DatabaseUserDataXmlMappingsString != DbUserMetaMappingsTextBox.Text);
            }
        }

        public bool SettingsModified
        {
            get
            {
                if (DbDriverComboBox.SelectedValue == null)
                    return false;

                return (DefaultSettings.Instance.DbDriver != DbDriverComboBox.SelectedValue.ToString()) ||
                       (DefaultSettings.Instance.ConnectionString != ConnectionStringTextBox.Text) ||
                       (DefaultSettings.Instance.LanguageMappingFile != LanguageFileTextBox.Text) ||
                       (DefaultSettings.Instance.Language != LanguageComboBox.Text) ||
                       (DefaultSettings.Instance.DbTargetMappingFile != DbTargetFileTextBox.Text) ||
                       (DefaultSettings.Instance.DbTarget != TargetDbComboBox.Text) ||
                       (DefaultSettings.Instance.UserMetaDataFileName != UserMetaDataFileTextBox.Text);
            }
        }

        private void ReloadSavedDbConnections()
        {
            SavedConnectionComboBox.Items.Clear();
            foreach (ConnectionInfo info in DefaultSettings.Instance.SavedConnections.Values)
            {
                SavedConnectionComboBox.Items.Add(info);
            }
            SavedConnectionComboBox.Sorted = true;
            SavedConnectionComboBox.SelectedIndex = -1;
        }

        private void PopulateLanguages()
        {
            LanguageComboBox.Items.Clear();
            LanguageComboBox.SelectedText = "";

            string[] languages = _myMeta.GetLanguageMappings(DbDriverComboBox.SelectedValue as string);

            if (null != languages)
            {
                foreach (string language in languages)
                {
                    LanguageComboBox.Items.Add(language);
                }
            }
        }

        private void PopulateDbTargets()
        {
            TargetDbComboBox.Items.Clear();
            TargetDbComboBox.SelectedText = "";

            string[] targets = _myMeta.GetDbTargetMappings(DbDriverComboBox.SelectedValue as string);

            if (null != targets)
            {
                foreach (string target in targets)
                {
                    TargetDbComboBox.Items.Add(target);
                }
            }
        }

        private string PickFile(string filter)
        {
            openFileDialog.InitialDirectory = Application.StartupPath + @"\Settings";
            openFileDialog.Filter = filter;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            else return string.Empty;
        }

        private void BindControlsToSettings()
        {
            DefaultSettings.Instance.DbDriver = DbDriverComboBox.SelectedValue as string;
            DefaultSettings.Instance.ConnectionString = ConnectionStringTextBox.Text;
            DefaultSettings.Instance.LanguageMappingFile = LanguageFileTextBox.Text;
            DefaultSettings.Instance.Language = LanguageComboBox.SelectedItem as string;
            DefaultSettings.Instance.DbTargetMappingFile = DbTargetFileTextBox.Text;
            DefaultSettings.Instance.DbTarget = TargetDbComboBox.SelectedItem as string;
            DefaultSettings.Instance.UserMetaDataFileName = UserMetaDataFileTextBox.Text;
            DefaultSettings.Instance.EnableClipboard = CopyOutputToClipboardCheckBox.Checked;
            DefaultSettings.Instance.ExecuteFromTemplateBrowserAsync = RunTemplatesAsyncCheckBox.Checked;
            DefaultSettings.Instance.ShowDefaultDatabaseOnly = ShowDefaultDbOnlyCheckBox.Checked;
            DefaultSettings.Instance.ConsoleWriteGeneratedDetails = ShowConsoleOutputCheckBox.Checked;
            DefaultSettings.Instance.EnableDocumentStyleSettings = DocumentStyleSettingsCheckBox.Checked;
            DefaultSettings.Instance.EnableLineNumbering = ShowLineNumbersCheckBox.Checked;
            DefaultSettings.Instance.Tabs = Convert.ToInt32(TabSizeTextBox.Text);
            DefaultSettings.Instance.CheckForNewBuild = CheckForUpdatesCheckBox.Checked;
            DefaultSettings.Instance.DomainOverride = DomainOverrideCheckBox.Checked;

            DefaultSettings.Instance.DatabaseUserDataXmlMappingsString = DbUserMetaMappingsTextBox.Text;

            if (CodePageComboBox.SelectedIndex > 0)
            {
                string selText = CodePageComboBox.SelectedItem.ToString();
                DefaultSettings.Instance.CodePage = Int32.Parse(selText.Substring(0, selText.IndexOf(':')));
            }
            else
            {
                DefaultSettings.Instance.CodePage = -1;
            }
            if (FontTextBox.Text.Trim() != string.Empty)
            {
                try
                {
                    Font f = new Font(FontTextBox.Text, 12);
                    DefaultSettings.Instance.FontFamily = f.FontFamily.Name;
                }
                catch { }
            }
            else
            {
                DefaultSettings.Instance.FontFamily = string.Empty;
            }

            DefaultSettings.Instance.DefaultTemplateDirectory = DefaultTemplatePathTextBox.Text;
            DefaultSettings.Instance.DefaultOutputDirectory = OutputPathTextBox.Text;

            if (checkBoxDisableTimeout.Checked)
            {
                DefaultSettings.Instance.ScriptTimeout = -1;
            }
            else
            {
                DefaultSettings.Instance.ScriptTimeout = Convert.ToInt32(textBoxTimeout.Text);
            }

            DefaultSettings.Instance.UseProxyServer = UseProxyServerCheckBox.Checked;
            DefaultSettings.Instance.ProxyServerUri = ProxyServerTextBox.Text;
            DefaultSettings.Instance.ProxyAuthUsername = ProxyUserTextBox.Text;
            DefaultSettings.Instance.ProxyAuthPassword = ProxyPasswordTextBox.Text;
            DefaultSettings.Instance.ProxyAuthDomain = ProxyDomainTextBox.Text;

            DefaultSettings.Instance.FirstLoad = false;

            InternalDriver drv = InternalDriver.Get(DefaultSettings.Instance.DbDriver);
            if (drv != null)
            {
                drv.ConnectString = ConnectionStringTextBox.Text;
                DefaultSettings.Instance.SetSetting(drv.DriverId, ConnectionStringTextBox.Text);
            }
            else
            {
                MessageBox.Show(this, "Choosing '<None>' will eliminate your ability to run 99.9% of the MyGeneration Templates. Most templates will crash if you run them",
                    "Warning !!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        public string TextContent
        {
            get { return DefaultSettings.Instance.ConnectionString; }
        }

        private bool DbConnectionOk(bool silent)
        {
            var dbDriver = string.Empty;
            var connectionString = string.Empty;

            try
            {
                if (DbDriverComboBox.SelectedValue != null) dbDriver = DbDriverComboBox.SelectedValue as string;
                connectionString = ConnectionStringTextBox.Text;

                if (string.IsNullOrWhiteSpace(dbDriver))
                {
                    MessageBox.Show("You must choose a DB driver", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (connectionString == string.Empty && dbDriver != "NONE")
                {
                    MessageBox.Show("Please enter a connection string", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                if (dbDriver.ToUpper() != "NONE")
                {
                    return DbConnectionOk(dbDriver, connectionString, silent);
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Unable to connect", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool DbConnectionOk(string dbDriver, string connectionString, bool silent)
        {
            var testConnectionForm = new TestConnectionForm(dbDriver, connectionString, silent);
            testConnectionForm.ShowDialog(this);

            return TestConnectionForm.State != ConnectionTestState.Error;
        }

        public DataTable DriversTable
        {
            get
            {
                if (_driversTable == null)
                {
                    _driversTable = new DataTable();
                    _driversTable.Columns.Add("DISPLAY");
                    _driversTable.Columns.Add("VALUE");
                    _driversTable.Columns.Add("ISPLUGIN");
                    _driversTable.Rows.Add(new object[] { "<None>", "NONE", false });
                    _driversTable.Rows.Add(new object[] { "Advantage Database Server", "ADVANTAGE", false });
                    _driversTable.Rows.Add(new object[] { "Firebird", "FIREBIRD", false });
                    _driversTable.Rows.Add(new object[] { "IBM DB2", "DB2", false });
                    _driversTable.Rows.Add(new object[] { "IBM iSeries (AS400)", "ISERIES", false });
                    _driversTable.Rows.Add(new object[] { "Interbase", "INTERBASE", false });
                    _driversTable.Rows.Add(new object[] { "Microsoft SQL Server", "SQL", false });
                    _driversTable.Rows.Add(new object[] { "Microsoft Access", "ACCESS", false });
                    _driversTable.Rows.Add(new object[] { "MySQL", "MYSQL", false });
                    _driversTable.Rows.Add(new object[] { "MySQL2", "MYSQL2", false });
                    _driversTable.Rows.Add(new object[] { "Oracle", "ORACLE", false });
                    _driversTable.Rows.Add(new object[] { "Pervasive", "PERVASIVE", false });
                    _driversTable.Rows.Add(new object[] { "PostgreSQL", "POSTGRESQL", false });
                    _driversTable.Rows.Add(new object[] { "PostgreSQL 8+", "POSTGRESQL8", false });
                    _driversTable.Rows.Add(new object[] { "SQLite", "SQLITE", false });
#if !IGNORE_VISTA
                    _driversTable.Rows.Add(new object[] { "VistaDB", "VISTADB", false });
#endif

                    foreach (IMyMetaPlugin plugin in MyMeta.dbRoot.Plugins.Values)
                    {
                        _driversTable.Rows.Add(new object[] { plugin.ProviderName, plugin.ProviderUniqueKey, true });
                    }

                    _driversTable.DefaultView.Sort = "DISPLAY";
                }
                return _driversTable;
            }
        }

        #region Control Event Handlers
        private void SettingsCancelButton_OnClicked(object sender, EventArgs e)
        {
            ParentForm.Close();
        }

        private void SettingsSaveButton_OnClicked(object sender, EventArgs e)
        {
            if (Save())
            {
                _mdi.SendAlert(ParentForm as IMyGenContent, "UpdateDefaultSettings");
                OnAfterSave();
            }
        }

        private void OleDbButton_OnClick(object sender, System.EventArgs e)
        {
            try
            {
                var dbDriver = string.Empty;
                var connectionString = string.Empty;

                if (DbDriverComboBox.SelectedValue != null) dbDriver = DbDriverComboBox.SelectedValue as string;
                connectionString = ConnectionStringTextBox.Text;

                if (string.IsNullOrWhiteSpace(dbDriver))
                {
                    MessageBox.Show("You Must Choose Driver", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                dbDriver = dbDriver.ToUpper();
                InternalDriver drv = InternalDriver.Get(dbDriver);
                if (ShowOleDbDialog != null)
                    drv.ShowOLEDBDialog = new ShowOleDbDialogHandler(ShowOleDbDialog);
                if (drv != null)
                {
                    if (string.Empty == connectionString)
                    {
                        connectionString = (drv != null) ? drv.ConnectString : string.Empty;
                    }

                    connectionString = drv.BrowseConnectionString(connectionString);
                    if (connectionString == null) return;
                    try
                    {
                        if (DbConnectionOk(dbDriver, connectionString, silent: true))
                        {
                            ConnectionStringTextBox.Text = connectionString;
                            return;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            catch { }
        }

        private void btnLanguageFile_Click(object sender, System.EventArgs e)
        {
            string fileName = PickFile("Langauge File (*.xml)|*.xml");

            if (string.Empty != fileName)
            {
                LanguageFileTextBox.Text = fileName;
                _myMeta.LanguageMappingFileName = fileName;
                PopulateLanguages();
            }
        }

        private void btnDbTargetFile_Click(object sender, System.EventArgs e)
        {
            string fileName = PickFile("Database Target File (*.xml)|*.xml");

            if (string.Empty != fileName)
            {
                DbTargetFileTextBox.Text = fileName;
                _myMeta.DbTargetMappingFileName = fileName;
                PopulateDbTargets();
            }
        }

        //		private string sqlString = @"Provider=SQLOLEDB.1;Persist Security Info=True;User ID=sa;Data Source=localhost";
        //		private string mySqlString = @"Provider=MySQLProv;Persist Security Info=True;Data Source=test;UID=griffo;PWD=;PORT=3306";
        //		private string accessString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=c:\access\newnorthwind.mdb;User Id=;Password=";
        //		private string oracleString = @"Provider=OraOLEDB.Oracle.1;Password=sa;Persist Security Info=True;User ID=GRIFFO;Data Source=dbMeta";
        //		private string db2String = @"Provider=IBMDADB2.1;Password=sa;User ID=DB2Admin;Data Source=MyMeta;Persist Security Info=True";
        //      private string firbirdstring = @"Provider=LCPI.IBProvider.2;Password=MyGen;Persist Security Info=True;User ID=sysdba;Data Source=MyGeneration;Location=c:\firebird\MyGeneration.gdb;ctype=;";
        private void cboDbDriver_SelectionChangeCommitted(object sender, System.EventArgs e)
        {
            ConnectionStringTextBox.Text = string.Empty;
            ConnectionStringTextBox.Enabled = true;

            LanguageComboBox.Enabled = true;
            TargetDbComboBox.Enabled = true;

            //			string language = LanguageComboBox.SelectedItem as string;
            //			string dbTarget = DbDriverComboBox.SelectedItem as string;

            PopulateLanguages();
            PopulateDbTargets();

            //			if(-1 != LanguageComboBox.FindStringExact(language))
            //			{
            //				LanguageComboBox.SelectedItem = language;
            //			}
            //
            //			if(-1 != DbDriverComboBox.FindStringExact(dbTarget))
            //			{
            //				DbDriverComboBox.SelectedItem = dbTarget;
            //			}

            string driver = string.Empty;
            if (null != DbDriverComboBox.SelectedValue) driver = DbDriverComboBox.SelectedValue as string;
            driver = driver.ToUpper();

            OleDbButton.BackColor = _defaultOleDbButtonColor;
            OleDbButton.Enabled = true;
            btnTestConnection.Enabled = true;

            InternalDriver drv = InternalDriver.Get(driver);
            if (drv != null)
            {
                bool oleDB = drv.IsOleDB;
                OleDbButton.Enabled = oleDB;
                if (oleDB)
                {
                    OleDbButton.BackColor = System.Drawing.Color.LightBlue;
                }

                ConnectionStringTextBox.Text = DefaultSettings.Instance.GetSetting(drv.DriverId, drv.ConnectString);
            }
            else
            {
                btnTestConnection.Enabled = false;
                //MessageBox.Show(this, "Choosing '<None>' will eliminate your ability to run 99.9% of the MyGeneration Templates. Most templates will crash if you run them",
                //    "Warning !!!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }

        }

        private void btnBrowse_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = DefaultSettings.Instance.DefaultTemplateDirectory;
            folderDialog.Description = "Select Default Template Directory";
            folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderDialog.ShowNewFolderButton = true;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                DefaultSettings.Instance.DefaultTemplateDirectory = folderDialog.SelectedPath;
                DefaultTemplatePathTextBox.Text = DefaultSettings.Instance.DefaultTemplateDirectory;
            }
        }

        private void buttonBrowseOutPath_Click(object sender, System.EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.SelectedPath = DefaultSettings.Instance.DefaultOutputDirectory;
            folderDialog.Description = "Select Default Output Directory";
            folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
            folderDialog.ShowNewFolderButton = true;

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                DefaultSettings.Instance.DefaultOutputDirectory = folderDialog.SelectedPath;
                OutputPathTextBox.Text = DefaultSettings.Instance.DefaultOutputDirectory;
            }
        }

        private void checkBoxDisableTimeout_CheckedChanged(object sender, System.EventArgs e)
        {
            bool isChecked = checkBoxDisableTimeout.Checked;

            if (isChecked)
            {
                textBoxTimeout.Text = "";
            }

            textBoxTimeout.Enabled = !isChecked;
            textBoxTimeout.ReadOnly = isChecked;
            label9.Enabled = !isChecked;
        }

        private void checkBoxUseProxyServer_CheckedChanged(object sender, System.EventArgs e)
        {
            bool isChecked = UseProxyServerCheckBox.Checked;

            ProxyServerTextBox.Enabled = isChecked;
            ProxyServerTextBox.ReadOnly = !isChecked;
            labelProxyServer.Enabled = isChecked;

            ProxyUserTextBox.Enabled = isChecked;
            ProxyUserTextBox.ReadOnly = !isChecked;
            labelProxyUser.Enabled = isChecked;

            ProxyPasswordTextBox.Enabled = isChecked;
            ProxyPasswordTextBox.ReadOnly = !isChecked;
            labelProxyPassword.Enabled = isChecked;

            ProxyDomainTextBox.Enabled = isChecked;
            ProxyDomainTextBox.ReadOnly = !isChecked;
            labelProxyDomain.Enabled = isChecked;
        }

        private void btnTestConnection_Click(object sender, System.EventArgs e)
        {
            DbConnectionOk(false);
        }

        private void cboDbDriver_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            cboDbDriver_SelectionChangeCommitted(sender, e);
        }

        private void buttonSave_Click(object sender, System.EventArgs e)
        {
            string text = SavedConnectionComboBox.Text.Trim();
            if (text == string.Empty)
            {
                MessageBox.Show("Please Enter a Name for this Saved Connection. Type the Name Directly into the ComboBox.", "Saved Connection Name Required", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                SavedConnectionComboBox.Focus();
            }
            else
            {
                _lastLoadedConnection = text;

                ConnectionInfo info = null;
                info = DefaultSettings.Instance.SavedConnections[text] as ConnectionInfo;

                if (info == null)
                {
                    info = new ConnectionInfo();
                    info.Name = SavedConnectionComboBox.Text;
                    DefaultSettings.Instance.SavedConnections[info.Name] = info;
                }

                info.Driver = DbDriverComboBox.SelectedValue.ToString();
                info.ConnectionString = ConnectionStringTextBox.Text;
                info.UserMetaDataPath = UserMetaDataFileTextBox.Text;
                info.Language = LanguageComboBox.Text;
                info.DbTarget = TargetDbComboBox.Text;
                info.LanguagePath = LanguageFileTextBox.Text;
                info.DbTargetPath = DbTargetFileTextBox.Text;
                info.DatabaseUserDataXmlMappingsString = DbUserMetaMappingsTextBox.Text;

                ReloadSavedDbConnections();
            }
        }

        private void buttonLoad_Click(object sender, System.EventArgs e)
        {
            ConnectionInfo info = SavedConnectionComboBox.SelectedItem as ConnectionInfo;

            if (info != null)
            {
                _lastLoadedConnection = info.Name;
                DefaultSettings.Instance.DbDriver = info.Driver;
                DefaultSettings.Instance.ConnectionString = info.ConnectionString;
                DefaultSettings.Instance.UserMetaDataFileName = info.UserMetaDataPath;
                DefaultSettings.Instance.Language = info.Language;
                DefaultSettings.Instance.LanguageMappingFile = info.LanguagePath;
                DefaultSettings.Instance.DbTarget = info.DbTarget;
                DefaultSettings.Instance.DbTargetMappingFile = info.DbTargetPath;

                DbDriverComboBox.SelectedValue = DefaultSettings.Instance.DbDriver;
                ConnectionStringTextBox.Text = DefaultSettings.Instance.ConnectionString;
                LanguageFileTextBox.Text = DefaultSettings.Instance.LanguageMappingFile;
                DbTargetFileTextBox.Text = DefaultSettings.Instance.DbTargetMappingFile;
                UserMetaDataFileTextBox.Text = DefaultSettings.Instance.UserMetaDataFileName;

                _myMeta.LanguageMappingFileName = DefaultSettings.Instance.LanguageMappingFile;
                _myMeta.DbTargetMappingFileName = DefaultSettings.Instance.DbTargetMappingFile;

                LanguageComboBox.Enabled = true;
                TargetDbComboBox.Enabled = true;

                PopulateLanguages();
                PopulateDbTargets();

                LanguageComboBox.SelectedItem = DefaultSettings.Instance.Language;
                TargetDbComboBox.SelectedItem = DefaultSettings.Instance.DbTarget;

                DbUserMetaMappingsTextBox.Text = info.DatabaseUserDataXmlMappingsString;

                DefaultSettings.Instance.DatabaseUserDataXmlMappings.Clear();
                foreach (string key in info.DatabaseUserDataXmlMappings.Keys)
                {
                    DefaultSettings.Instance.DatabaseUserDataXmlMappings[key] = info.DatabaseUserDataXmlMappings[key];
                }
            }
            else
            {
                MessageBox.Show("You Must Select a Saved Connection to Load", "No Saved Connection Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonDelete_Click(object sender, System.EventArgs e)
        {
            ConnectionInfo info = SavedConnectionComboBox.SelectedItem as ConnectionInfo;

            if (info != null)
            {
                DefaultSettings.Instance.SavedConnections.Remove(info.Name);

                ReloadSavedDbConnections();

                if (SavedConnectionComboBox.Items.Count > 0)
                    SavedConnectionComboBox.SelectedIndex = 0;
                else
                {
                    SavedConnectionComboBox.SelectedIndex = -1;
                    SavedConnectionComboBox.Text = string.Empty;
                }
            }
            else
            {
                MessageBox.Show("You Must Select a Saved Connection to Delete", "No Saved Connection Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void buttonFont_Click(object sender, EventArgs e)
        {
            try
            {
                Font f = new Font(FontTextBox.Text, 12);
                fontDialog1.Font = f;
            }
            catch
            {
                FontTextBox.Text = string.Empty;
            }

            fontDialog1.ShowColor = false;
            fontDialog1.ShowEffects = false;
            if (fontDialog1.ShowDialog() == DialogResult.OK)
            {
                FontTextBox.Text = fontDialog1.Font.FontFamily.Name;
            }
            else if (fontDialog1.ShowDialog() == DialogResult.None)
            {
                FontTextBox.Text = string.Empty;
            }
        }

        private void btnUserMetaDataFile_Click(object sender, System.EventArgs e)
        {
            string fileName = PickFile("Langauge File (*.xml)|*.xml");

            if (string.Empty != fileName)
            {
                UserMetaDataFileTextBox.Text = fileName;
            }
        }
#endregion
    }
}
