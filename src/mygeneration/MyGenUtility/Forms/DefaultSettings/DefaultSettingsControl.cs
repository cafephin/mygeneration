using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyGeneration.Configuration;
using MyMeta;

namespace MyGeneration
{
    public partial class DefaultSettingsControl : UserControl
    {
        private string _selectedSavedConnectionName = string.Empty;
        private dbRoot _myMeta;
        private IMyGenerationMDI _mdi;

        public ShowOleDbDialogHandler ShowOleDbDialog;
        public event EventHandler AfterSave;

        private DataTable _driversTable;
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
                    _driversTable.Rows.Add("<None>", "NONE", false);
                    _driversTable.Rows.Add("Advantage Database Server", "ADVANTAGE", false);
                    _driversTable.Rows.Add("Firebird", "FIREBIRD", false);
                    _driversTable.Rows.Add("IBM DB2", "DB2", false);
                    _driversTable.Rows.Add("IBM iSeries (AS400)", "ISERIES", false);
                    _driversTable.Rows.Add("Interbase", "INTERBASE", false);
                    _driversTable.Rows.Add("Microsoft SQL Server", "SQL", false);
                    _driversTable.Rows.Add("Microsoft Access", "ACCESS", false);
                    _driversTable.Rows.Add("MySQL", "MYSQL", false);
                    _driversTable.Rows.Add("MySQL2", "MYSQL2", false);
                    _driversTable.Rows.Add("Oracle", "ORACLE", false);
                    _driversTable.Rows.Add("Pervasive", "PERVASIVE", false);
                    _driversTable.Rows.Add("PostgreSQL", "POSTGRESQL", false);
                    _driversTable.Rows.Add("PostgreSQL 8+", "POSTGRESQL8", false);
                    _driversTable.Rows.Add("SQLite", "SQLITE", false);
                    _driversTable.Rows.Add("VistaDB", "VISTADB", false);

                    foreach (IMyMetaPlugin plugin in dbRoot.Plugins.Values)
                    {
                        _driversTable.Rows.Add(plugin.ProviderName, plugin.ProviderUniqueKey, true);
                    }

                    _driversTable.DefaultView.Sort = "DISPLAY";
                }
                return _driversTable;
            }
        }

        private string _dbDriver;

        public string DbDriver
        {
            get { return _dbDriver; }
            set
            {
                _dbDriver = value;
            }
        }

        public bool SaveSettings()
        {
            Configuration.DefaultSettings.Instance.DbConnectionSettings.Driver = DbDriver;
            return true;
        }

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
            PopulateConnectionSettings();
            PopulateTemplateSettings();
            PopulateMiscSettings();
        }

        private void PopulateConnectionSettings()
        {
            _myMeta = new dbRoot
                      {
                          ShowDefaultDatabaseOnly = DefaultSettings.Instance.DbConnectionSettings.ShowDefaultDatabaseOnly,
                          LanguageMappingFileName = DefaultSettings.Instance.DbConnectionSettings.LanguageMappingFile,
                          DbTargetMappingFileName = DefaultSettings.Instance.DbConnectionSettings.DbTargetMappingFile
                      };

            ReloadSavedDbConnections();
            PopulateConnectionStringSettings();

            PopulateDbTargetSettings();
            PopulateLanguageSettings();

            DbUserMetaMappingsTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.UserDatabaseAliasesDisplay;
            UserMetaDataFileTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.UserMetaDataFileName;
        }

        private void ReloadSavedDbConnections()
        {
            //SavedConnectionComboBox.Items.Clear();
            //foreach (ConnectionInfo info in DefaultSettings.Instance.SavedConnections.Values)
            //{
            //    SavedConnectionComboBox.Items.Add(info);
            //}
            SavedConnectionComboBox.Sorted = true;
            //SavedConnectionComboBox.SelectedIndex = -1;
            
            SavedConnectionComboBox.DataSource = null;
            SavedConnectionComboBox.DataSource = DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection;
            SavedConnectionComboBox.DisplayMember = "Name";
            SavedConnectionComboBox.ValueMember = "Name";
            var selectedSavedConnection = DefaultSettings.Instance
                                                         .DbConnectionSettings
                                                         .ConnectionInfoCollection
                                                         .FirstOrDefault(c => c.Name == _selectedSavedConnectionName);
            SavedConnectionComboBox.SelectedValue = selectedSavedConnection != null ? selectedSavedConnection.Name : string.Empty;
        }

        private void PopulateMiscSettings()
        {
            CheckForUpdatesCheckBox.Checked = DefaultSettings.Instance.MiscSettings.CheckForNewBuild;
            DomainOverrideCheckBox.Checked = DefaultSettings.Instance.MiscSettings.DomainOverride;
            ShowConsoleOutputCheckBox.Checked = DefaultSettings.Instance.MiscSettings.ConsoleWriteGeneratedDetails;
            DocumentStyleSettingsCheckBox.Checked = DefaultSettings.Instance.MiscSettings.EnableDocumentStyleSettings;
        }

        private void PopulateLanguageSettings()
        {
            PopulateLanguages();
            LanguageComboBox.Enabled = true;
            LanguageComboBox.SelectedItem = DefaultSettings.Instance.DbConnectionSettings.Language;
            LanguageFileTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.LanguageMappingFile;
        }

        private void PopulateDbTargetSettings()
        {
            PopulateDbTargets();
            TargetDbComboBox.Enabled = true;
            TargetDbComboBox.SelectedItem = DefaultSettings.Instance.DbConnectionSettings.DbTarget;
            DbTargetFileTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.DbTargetMappingFile;
        }

        private void PopulateTemplateSettings()
        {
            CopyOutputToClipboardCheckBox.Checked = DefaultSettings.Instance.TemplateSettings.EnableClipboard;
            RunTemplatesAsyncCheckBox.Checked = DefaultSettings.Instance.TemplateSettings.ExecuteFromTemplateBrowserAsync;
            
            
            ShowLineNumbersCheckBox.Checked = DefaultSettings.Instance.TemplateSettings.EnableLineNumbering;
            TabSizeTextBox.Text = DefaultSettings.Instance.TemplateSettings.TabSize.ToString();
            DefaultTemplatePathTextBox.Text = DefaultSettings.Instance.TemplateSettings.DefaultTemplateDirectory;
            OutputPathTextBox.Text = DefaultSettings.Instance.TemplateSettings.DefaultOutputDirectory;
            FontTextBox.Text = DefaultSettings.Instance.TemplateSettings.TemplateEditorFontFamily;
            PopulateTimeoutSettings();
            PopulateProxySettings();
            PopulateCodePageEncodingSettings();
        }

        private void PopulateConnectionStringSettings()
        {
            ShowDefaultDbOnlyCheckBox.Checked = DefaultSettings.Instance.DbConnectionSettings.ShowDefaultDatabaseOnly;

            DbDriverComboBox.DisplayMember = "DISPLAY";
            DbDriverComboBox.ValueMember = "VALUE";
            DbDriverComboBox.DataSource = DriversTable;
            DbDriverComboBox.SelectedValue = DefaultSettings.Instance.DbConnectionSettings.Driver;

            switch (DefaultSettings.Instance.DbConnectionSettings.Driver)
            {
                case "PERVASIVE":
                case "POSTGRESQL":
                case "POSTGRESQL8":
                case "FIREBIRD":
                case "INTERBASE":
                case "SQLITE":
                case "MYSQL2":
                case "VISTADB":
                case "ISERIES":
                case "NONE":
                case "":
                    OleDbButton.Enabled = false;
                    break;
            }

            ConnectionStringTextBox.Enabled = true;
            ConnectionStringTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.ConnectionString;
        }

        private void PopulateTimeoutSettings()
        {
            var timeout = DefaultSettings.Instance.TemplateSettings.ScriptTimeout;
            if (timeout == -1)
            {
                DisableTimeoutCheckBox.Checked = true;
                DisableTimeoutCheckBox_OnCheckedStateChanged(this, new EventArgs());
            }
            else
            {
                DisableTimeoutCheckBox.Checked = false;
                DisableTimeoutCheckBox_OnCheckedStateChanged(this, new EventArgs());
                TimeoutTextBox.Text = timeout.ToString();
            }
        }

        private void PopulateCodePageEncodingSettings()
        {
            var encodings = Encoding.GetEncodings();
            CodePageComboBox.Items.Add(string.Empty);
            foreach (EncodingInfo encoding in encodings)
            {
                var windowsCodePage = encoding.CodePage + ": " + encoding.DisplayName;
                var idx = CodePageComboBox.Items.Add(windowsCodePage);

                if (encoding.CodePage == DefaultSettings.Instance.TemplateSettings.CodePageEncodingId)
                {
                    CodePageComboBox.SelectedIndex = idx;
                }
            }
        }

        private void PopulateProxySettings()
        {
            UseProxyServerCheckBox.Checked = DefaultSettings.Instance.TemplateSettings.UseProxyServer;
            ProxyServerTextBox.Text = DefaultSettings.Instance.TemplateSettings.ProxyServerUri;
            ProxyUserTextBox.Text = DefaultSettings.Instance.TemplateSettings.ProxyAuthUsername;
            ProxyPasswordTextBox.Text = DefaultSettings.Instance.TemplateSettings.ProxyAuthPassword;
            ProxyDomainTextBox.Text = DefaultSettings.Instance.TemplateSettings.ProxyAuthDomain;
        }

        public bool Save()
        {
            BindControlsToSettings();
            DefaultSettings.Instance.Save(DefaultSettings.Instance.SettingsFilename);
            return true;
        }

        protected void OnAfterSave()
        {
            if (AfterSave != null) AfterSave.Invoke(this, EventArgs.Empty);
        }

        public void Cancel()
        {
            DefaultSettings.Instance.DiscardChanges();
        }

        public bool ConnectionInfoModified
        {
            get
            {
                var connectionInfo =
                    DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection
                                   .FirstOrDefault(c => c.Name == _selectedSavedConnectionName);

                if (_selectedSavedConnectionName == string.Empty ||
                    DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection.Any(c => c.Name == _selectedSavedConnectionName))
                {
                    return false;
                }

                return connectionInfo.DbDriver != DbDriverComboBox.SelectedValue.ToString() ||
                       connectionInfo.ConnectionString != ConnectionStringTextBox.Text ||
                       connectionInfo.LanguagePath != LanguageFileTextBox.Text ||
                       connectionInfo.Language != LanguageComboBox.Text ||
                       connectionInfo.DbTargetPath != DbTargetFileTextBox.Text ||
                       connectionInfo.DbTarget != TargetDbComboBox.Text ||
                       connectionInfo.UserMetaDataPath != UserMetaDataFileTextBox.Text ||
                       connectionInfo.DatabaseUserDataXmlMappingsString != DbUserMetaMappingsTextBox.Text;
            }
        }

        public bool SettingsModified
        {
            get
            {
                if (DbDriverComboBox.SelectedValue == null)
                    return false;

                return DefaultSettings.Instance.DbConnectionSettings.Driver != DbDriverComboBox.SelectedValue.ToString() ||
                       DefaultSettings.Instance.DbConnectionSettings.ConnectionString != ConnectionStringTextBox.Text ||
                       DefaultSettings.Instance.DbConnectionSettings.LanguageMappingFile != LanguageFileTextBox.Text ||
                       DefaultSettings.Instance.DbConnectionSettings.Language != LanguageComboBox.Text ||
                       DefaultSettings.Instance.DbConnectionSettings.DbTargetMappingFile != DbTargetFileTextBox.Text ||
                       DefaultSettings.Instance.DbConnectionSettings.DbTarget != TargetDbComboBox.Text ||
                       DefaultSettings.Instance.DbConnectionSettings.UserMetaDataFileName != UserMetaDataFileTextBox.Text;
            }
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
            return string.Empty;
        }

        private void BindControlsToSettings()
        {
            DefaultSettings.Instance.DbConnectionSettings.Driver = DbDriverComboBox.SelectedValue as string;
            DefaultSettings.Instance.DbConnectionSettings.ConnectionString = ConnectionStringTextBox.Text;
            DefaultSettings.Instance.DbConnectionSettings.LanguageMappingFile = LanguageFileTextBox.Text;
            DefaultSettings.Instance.DbConnectionSettings.Language = LanguageComboBox.SelectedItem as string;
            DefaultSettings.Instance.DbConnectionSettings.DbTargetMappingFile = DbTargetFileTextBox.Text;
            DefaultSettings.Instance.DbConnectionSettings.DbTarget = TargetDbComboBox.SelectedItem as string;
            DefaultSettings.Instance.DbConnectionSettings.UserMetaDataFileName = UserMetaDataFileTextBox.Text;
            DefaultSettings.Instance.DbConnectionSettings.ShowDefaultDatabaseOnly = ShowDefaultDbOnlyCheckBox.Checked;

            DefaultSettings.Instance.TemplateSettings.EnableClipboard = CopyOutputToClipboardCheckBox.Checked;
            DefaultSettings.Instance.TemplateSettings.ExecuteFromTemplateBrowserAsync = RunTemplatesAsyncCheckBox.Checked;
            DefaultSettings.Instance.TemplateSettings.EnableLineNumbering = ShowLineNumbersCheckBox.Checked;
            DefaultSettings.Instance.TemplateSettings.TabSize = Convert.ToInt32(TabSizeTextBox.Text);

            DefaultSettings.Instance.MiscSettings.CheckForNewBuild = CheckForUpdatesCheckBox.Checked;
            DefaultSettings.Instance.MiscSettings.DomainOverride = DomainOverrideCheckBox.Checked;
            DefaultSettings.Instance.MiscSettings.ConsoleWriteGeneratedDetails = ShowConsoleOutputCheckBox.Checked;
            DefaultSettings.Instance.MiscSettings.EnableDocumentStyleSettings = DocumentStyleSettingsCheckBox.Checked;

            DefaultSettings.Instance.DbConnectionSettings.UserDatabaseAliasesDisplay = DbUserMetaMappingsTextBox.Text;

            if (CodePageComboBox.SelectedIndex > 0)
            {
                var selText = CodePageComboBox.SelectedItem.ToString();
                DefaultSettings.Instance.TemplateSettings.CodePageEncodingId = int.Parse(selText.Substring(0, selText.IndexOf(':')));
            }
            else
            {
                DefaultSettings.Instance.TemplateSettings.CodePageEncodingId = -1;
            }

            if (FontTextBox.Text.Trim() != string.Empty)
            {
                try
                {
                    Font f = new Font(FontTextBox.Text, 12);
                    DefaultSettings.Instance.TemplateSettings.TemplateEditorFontFamily = f.FontFamily.Name;
                }
                catch { }
            }
            else
            {
                DefaultSettings.Instance.TemplateSettings.TemplateEditorFontFamily = string.Empty;
            }

            DefaultSettings.Instance.TemplateSettings.DefaultTemplateDirectory = DefaultTemplatePathTextBox.Text;
            DefaultSettings.Instance.TemplateSettings.DefaultOutputDirectory = OutputPathTextBox.Text;

            if (DisableTimeoutCheckBox.Checked)
            {
                DefaultSettings.Instance.TemplateSettings.ScriptTimeout = -1;
            }
            else
            {
                DefaultSettings.Instance.TemplateSettings.ScriptTimeout = Convert.ToInt32(TimeoutTextBox.Text);
            }

            DefaultSettings.Instance.TemplateSettings.UseProxyServer = UseProxyServerCheckBox.Checked;
            DefaultSettings.Instance.TemplateSettings.ProxyServerUri = ProxyServerTextBox.Text;
            DefaultSettings.Instance.TemplateSettings.ProxyAuthUsername = ProxyUserTextBox.Text;
            DefaultSettings.Instance.TemplateSettings.ProxyAuthPassword = ProxyPasswordTextBox.Text;
            DefaultSettings.Instance.TemplateSettings.ProxyAuthDomain = ProxyDomainTextBox.Text;

            InternalDriver internalDriver = InternalDriver.Get(DefaultSettings.Instance.DbConnectionSettings.Driver);
            if (internalDriver != null)
            {
                internalDriver.ConnectString = ConnectionStringTextBox.Text;
                //DefaultSettings.Instance.SetSetting(internalDriver.DriverId, ConnectionStringTextBox.Text);
                DefaultSettings.Instance.DbConnectionSettings.ConnectionString = ConnectionStringTextBox.Text;
            }
            else
            {
                MessageBox.Show(this,
                                "Choosing '<None>' will eliminate your ability to run 99.9% of the MyGeneration templates. " +
                                "Most templates will crash if you run them",
                                "Warning",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Stop);
            }
        }

        public string TextContent
        {
            get { return DefaultSettings.Instance.DbConnectionSettings.ConnectionString; }
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

        private void LanguageFileBrowseButton_OnClicked(object sender, System.EventArgs e)
        {
            string fileName = PickFile("Langauge File (*.xml)|*.xml");

            if (string.Empty != fileName)
            {
                LanguageFileTextBox.Text = fileName;
                _myMeta.LanguageMappingFileName = fileName;
                PopulateLanguages();
            }
        }

        private void DbTargetFileBrowseButton_OnClicked(object sender, EventArgs e)
        {
            string fileName = PickFile("Database Target File (*.xml)|*.xml");

            if (string.Empty != fileName)
            {
                DbTargetFileTextBox.Text = fileName;
                _myMeta.DbTargetMappingFileName = fileName;
                PopulateDbTargets();
            }
        }

        private void DbDriverComboBox_OnSelectionChanged(object sender, EventArgs e)
        {
            ConnectionStringTextBox.Text = string.Empty;
            ConnectionStringTextBox.Enabled = true;

            LanguageComboBox.Enabled = true;
            TargetDbComboBox.Enabled = true;

            PopulateLanguages();
            PopulateDbTargets();

            var dbDriver = string.Empty;
            if (DbDriverComboBox.SelectedValue != null) dbDriver = DbDriverComboBox.SelectedValue as string;
            dbDriver = dbDriver.ToUpper();

            OleDbButton.Enabled = true;
            TestConnectionButton.Enabled = true;

            InternalDriver internalDriver = InternalDriver.Get(dbDriver);
            if (internalDriver != null)
            {
                bool oleDB = internalDriver.IsOleDB;
                OleDbButton.Enabled = oleDB;
                if (oleDB)
                {
                    OleDbButton.BackColor = Color.LightBlue;
                }

                ConnectionStringTextBox.Text = internalDriver.ConnectString; //TODO
            }
            else
            {
                TestConnectionButton.Enabled = false;
            }
        }

        private void DefaultTemplatePathBrowseButton_OnClicked(object sender, System.EventArgs e)
        {
            var folderDialog = new FolderBrowserDialog
                               {
                                   SelectedPath = DefaultSettings.Instance.TemplateSettings.DefaultTemplateDirectory,
                                   Description = "Select Default Template Directory",
                                   RootFolder = Environment.SpecialFolder.MyComputer,
                                   ShowNewFolderButton = true
                               };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                DefaultSettings.Instance.TemplateSettings.DefaultTemplateDirectory = folderDialog.SelectedPath;
                DefaultTemplatePathTextBox.Text = DefaultSettings.Instance.TemplateSettings.DefaultTemplateDirectory;
            }
        }

        private void OutputPathBrowseButton_OnClicked(object sender, EventArgs e)
        {
            var folderDialog =
                new FolderBrowserDialog
                {
                    SelectedPath = DefaultSettings.Instance.TemplateSettings.DefaultOutputDirectory,
                    Description = "Select Default Output Directory",
                    RootFolder = Environment.SpecialFolder.MyComputer,
                    ShowNewFolderButton = true
                };

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                DefaultSettings.Instance.TemplateSettings.DefaultOutputDirectory = folderDialog.SelectedPath;
                OutputPathTextBox.Text = DefaultSettings.Instance.TemplateSettings.DefaultOutputDirectory;
            }
        }

        private void DisableTimeoutCheckBox_OnCheckedStateChanged(object sender, EventArgs e)
        {
            bool isChecked = DisableTimeoutCheckBox.Checked;

            if (isChecked)
            {
                TimeoutTextBox.Text = "";
            }

            TimeoutTextBox.Enabled = !isChecked;
            TimeoutTextBox.ReadOnly = isChecked;
            label9.Enabled = !isChecked;
        }

        private void UseProxyServerCheckBox_OnCheckedStateChanged(object sender, System.EventArgs e)
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

        private void TestConnectionButton_OnClicked(object sender, System.EventArgs e)
        {
            DbConnectionOk(false);
        }

        private void DbDriverComboBox_OnSelectedIndexChanged(object sender, System.EventArgs e)
        {
            DbDriverComboBox_OnSelectionChanged(sender, e);
        }

        private void SavedConnectionsSaveButton_OnClicked(object sender, EventArgs e)
        {
            var savedConnectionName = SavedConnectionComboBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(savedConnectionName))
            {
                MessageBox.Show("Please provide a name for the saved connection.",
                                "Saved Connection Name Required",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
                SavedConnectionComboBox.Focus();
                return;
            }

            _selectedSavedConnectionName = savedConnectionName;

            ConnectionInfo connectionInfo = null;
            if (DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection.Any(c => c.Name == savedConnectionName))
            {
                connectionInfo = DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection.FirstOrDefault(c => c.Name == savedConnectionName);
            }

            if (connectionInfo == null)
            {
                connectionInfo = new ConnectionInfo {Name = SavedConnectionComboBox.Text};
                DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection.Add(connectionInfo);
            }

            connectionInfo.DbDriver = DbDriverComboBox.SelectedValue.ToString();
            connectionInfo.ConnectionString = ConnectionStringTextBox.Text;
            connectionInfo.UserMetaDataPath = UserMetaDataFileTextBox.Text;
            connectionInfo.Language = LanguageComboBox.Text;
            connectionInfo.DbTarget = TargetDbComboBox.Text;
            connectionInfo.LanguagePath = LanguageFileTextBox.Text;
            connectionInfo.DbTargetPath = DbTargetFileTextBox.Text;
            connectionInfo.DatabaseUserDataXmlMappingsString = DbUserMetaMappingsTextBox.Text;

            ReloadSavedDbConnections();
        }

        private void SavedConnectionLoadButton_OnClicked(object sender, EventArgs e)
        {
            var connectionInfo = SavedConnectionComboBox.SelectedItem as ConnectionInfo;

            if (connectionInfo != null)
            {
                _selectedSavedConnectionName = connectionInfo.Name;
                DefaultSettings.Instance.DbConnectionSettings.Driver = connectionInfo.DbDriver;
                DefaultSettings.Instance.DbConnectionSettings.ConnectionString = connectionInfo.ConnectionString;
                DefaultSettings.Instance.DbConnectionSettings.UserMetaDataFileName = connectionInfo.UserMetaDataPath;
                DefaultSettings.Instance.DbConnectionSettings.Language = connectionInfo.Language;
                DefaultSettings.Instance.DbConnectionSettings.LanguageMappingFile = connectionInfo.LanguagePath;
                DefaultSettings.Instance.DbConnectionSettings.DbTarget = connectionInfo.DbTarget;
                DefaultSettings.Instance.DbConnectionSettings.DbTargetMappingFile = connectionInfo.DbTargetPath;

                DbDriverComboBox.SelectedValue = DefaultSettings.Instance.DbConnectionSettings.Driver;
                ConnectionStringTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.ConnectionString;
                LanguageFileTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.LanguageMappingFile;
                DbTargetFileTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.DbTargetMappingFile;
                UserMetaDataFileTextBox.Text = DefaultSettings.Instance.DbConnectionSettings.UserMetaDataFileName;

                _myMeta.LanguageMappingFileName = DefaultSettings.Instance.DbConnectionSettings.LanguageMappingFile;
                _myMeta.DbTargetMappingFileName = DefaultSettings.Instance.DbConnectionSettings.DbTargetMappingFile;

                LanguageComboBox.Enabled = true;
                TargetDbComboBox.Enabled = true;

                PopulateLanguages();
                PopulateDbTargets();

                LanguageComboBox.SelectedItem = DefaultSettings.Instance.DbConnectionSettings.Language;
                TargetDbComboBox.SelectedItem = DefaultSettings.Instance.DbConnectionSettings.DbTarget;

                DbUserMetaMappingsTextBox.Text = connectionInfo.DatabaseUserDataXmlMappingsString;

                DefaultSettings.Instance.DbConnectionSettings.UserDatabaseAliases.Clear();
                foreach (var databaseAlias in connectionInfo.UserDatabaseAliases)
                {
                    DefaultSettings.Instance.DbConnectionSettings.UserDatabaseAliases.Add(databaseAlias);
                }
            }
            else
            {
                MessageBox.Show("You must select a saved connection to load", "No Saved Connection Selected", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void SavedConnectionDeleteButton_OnClicked(object sender, EventArgs e)
        {
            var selectedSavedConnectionName = SavedConnectionComboBox.SelectedValue as string;

            if (DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection.Any(c => c.Name == selectedSavedConnectionName))
            {
                var index = DefaultSettings.Instance
                    .DbConnectionSettings
                    .ConnectionInfoCollection
                    .FindIndex(c => c.Name == selectedSavedConnectionName);
                DefaultSettings.Instance.DbConnectionSettings.ConnectionInfoCollection.RemoveAt(index);

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
                MessageBox.Show("You must select a saved connection to delete",
                                "No Saved Connection Selected",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Exclamation);
            }
        }

        private void FontButton_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var f = new Font(FontTextBox.Text, 12);
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

        private void UserMetadataFileBrowseButton_OnClicked(object sender, System.EventArgs e)
        {
            var fileName = PickFile("Language File (*.xml)|*.xml");

            if (!string.IsNullOrWhiteSpace(fileName))
            {
                UserMetaDataFileTextBox.Text = fileName;
            }
        }
        #endregion
    }
}
