using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using MyGeneration.Shared;
using Zeus;

namespace MyGeneration.Configuration
{
    public class DefaultSettings
	{
		private const string MISSING = "*&?$%";
	    private const int MAX_NUMBER_OF_RECENT_FILES = 20;

	    private Hashtable _savedConnections;
		private Dictionary<string, string> _databaseUserMetaMappings;
        private XmlElement _settingsRootNode;

        // Use "DefaultSettings.Settings" instead of "new DefaultSettings()" to get the settings
        // To discard changes without saving call settings.DiscardChanges()
		private DefaultSettings()
		{
            Load();
        }

	    private static string _applicationPath;
        public static string ApplicationPath
        {
            get 
            {
                if (_applicationPath == null)
                {
                    _applicationPath = Assembly.GetEntryAssembly().Location;
                    _applicationPath = _applicationPath.Substring(0, _applicationPath.LastIndexOf(@"\", StringComparison.Ordinal));
                }
                return _applicationPath; 
            }
            set { _applicationPath = value; }
        }

	    private static DefaultSettings _instance;

	    public static DefaultSettings Instance
	    {
	        get { return _instance ?? (_instance = new DefaultSettings()); }
	    }

	    // Defaults to user-directory\MyGeneration\DefaultSettings.xml if found 
        // Else app-directory\Settings\DefaultSettings.xml
	    private static string _settingsFileName = null;
        public static string SettingsFileName
        {
            get
            {
                if (_settingsFileName != null) return _settingsFileName;
                
                _settingsFileName = UserDataPath + @"\Settings\DefaultSettings.xml";
                return _settingsFileName;
            }
            set 
            {
                if (value == _settingsFileName) return;
                _settingsFileName = value; DefaultSettings.Refresh();
            }
        }

	    public static void Refresh()
	    {
	        _instance = null;
	    }

	    public void DiscardChanges()
	    {
	        _savedConnections = null;
	        _recentFiles = null;
	        _databaseUserMetaMappings = null;

	        Load();
	    }

        private void Load()
        {
            var xmlDoc = new XmlDocument();
            
            _settingsRootNode = null;
            try
            {
                var filename = SettingsFileName;
#if RUN_AS_NON_ADMIN
                // current user has no own settings yet
                if (!System.IO.File.Exists(filename))
                    filename = ApplicationPath + @"\Settings\DefaultSettings.xml";                
#endif
                xmlDoc.Load(filename);
            }
            catch
            {
                // Our file doesn't exist, or it is invalid xml. So let's (re-)create it
                xmlDoc = new XmlDocument();
                var defaultXML = new StringBuilder();
                defaultXML.Append(@"<?xml version='1.0' encoding='utf-8'?>");
                defaultXML.Append(@"<DefaultSettings>");
                defaultXML.Append(@"</DefaultSettings>");

                xmlDoc.LoadXml(defaultXML.ToString());
            }

            _settingsRootNode = xmlDoc.SelectSingleNode("//DefaultSettings", null) as XmlElement;
            if (_settingsRootNode == null)
            {
                _settingsRootNode = xmlDoc.AppendChild(xmlDoc.CreateElement("DefaultSettings", null)) as XmlElement;
            }

            Assembly myGenerationAssembly = Assembly.GetEntryAssembly();
            var version = myGenerationAssembly == null
                              ? "Unknown"
                              : (myGenerationAssembly.GetName().Version.ToString());
            if (Version != version)
            {
                // Our Version has changed, or DefaultSettings were just created
                // write any new settings and their defaults
                FillInMissingSettings(version);
                Save();
            }
        }
	
        private void FillInMissingSettings(string version)
        {
            Version = version;

            foreach (PropertyInfo prop in GetType().GetProperties())
            {
                MethodInfo getter = prop.GetGetMethod();
                MethodInfo setter = prop.GetSetMethod();

                if (getter != null && setter != null)
                {
                    var par = new object[1];
                    par[0] = getter.Invoke(this, null);
                    setter.Invoke(this, par);
                }
            }
        }

		public void Save()
		{
            UpdateSavedConnections();
            UpdateDatabaseMappings();
			UpdateRecentFiles();
            
			_settingsRootNode.OwnerDocument.Save(SettingsFileName);
		}

		#region Recent Files Nodes
		public void UpdateRecentFiles() 
		{
		    if (_recentFiles == null) return;

		    XmlNode parentNode = _settingsRootNode;

		    var nodesToHack = new ArrayList();
		    XmlNodeList nodes = _settingsRootNode.GetElementsByTagName("RecentFile");
				
		    foreach (XmlNode node in nodes) nodesToHack.Add(node);
		    foreach (XmlNode node in nodesToHack) parentNode.RemoveChild(node);

		    var i = 0;
		    foreach (string path in _recentFiles) 
		    {
		        XmlNode node = _settingsRootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "RecentFile", null);
		        node.InnerText = path;

		        parentNode.AppendChild(node);

		        if (++i > MAX_NUMBER_OF_RECENT_FILES) break;
		    }
		    _recentFiles = null;
		}

	    private ArrayList _recentFiles;
		public ArrayList RecentFiles
		{
			get 
			{
			    if (_recentFiles != null) return _recentFiles;

			    _recentFiles = new ArrayList();
                XmlNodeList nodes = _settingsRootNode.GetElementsByTagName("RecentFile");
			    foreach (XmlNode node in nodes)
			    {
			        if (string.IsNullOrEmpty(node.InnerText)) continue;
			        if (File.Exists(node.InnerText))
			        {
			            _recentFiles.Add(node.InnerText);
			        }
			    }

			    return _recentFiles; 
			}
		}
		#endregion

        #region Database Mappings
        public void UpdateDatabaseMappings()
        {
            if (_databaseUserMetaMappings != null)
            {
                XmlNode parentNode = _settingsRootNode;

                // Cleanup nodes
                XmlNodeList nodes = _settingsRootNode.GetElementsByTagName("XmlUserMetaDatabaseAlias");
                ArrayList nodesToHack = new ArrayList();
                foreach (XmlNode node in nodes) nodesToHack.Add(node);
                foreach (XmlNode node in nodesToHack) parentNode.RemoveChild(node);

                foreach (string dbkey in DatabaseUserDataXmlMappings.Keys)
                {
                    XmlNode mapXmlNode = parentNode.AppendChild(_settingsRootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "XmlUserMetaDatabaseAlias", null));

                    XmlHelper.SetAttribute(mapXmlNode, "database", dbkey);
                    XmlHelper.SetAttribute(mapXmlNode, "alias", DatabaseUserDataXmlMappings[dbkey]);
                }

                _databaseUserMetaMappings = null;
            }
        }
        #endregion

        #region Saved Connections
        public void UpdateSavedConnections() 
		{
		    if (_savedConnections == null) return;

		    XmlNode parentNode = _settingsRootNode;
		    XmlNodeList nodes = _settingsRootNode.GetElementsByTagName("SavedSettings");
		    var nodesToHack = new ArrayList();
		    foreach (XmlNode node in nodes) 
		    {
		        if (!SavedConnections.ContainsKey(XmlHelper.GetAttribute(node, "name", MISSING)))
		        {
		            nodesToHack.Add(node);
		        }
		    }
		    foreach (XmlNode node in nodesToHack) parentNode.RemoveChild(node);

		    foreach (ConnectionInfo inf in _savedConnections.Values) 
		    {
		        var xPath = @"SavedSettings[@name='" + inf.Name.Replace("'", "&apos;") + "']";
		        XmlNode connectionNode = _settingsRootNode.SelectSingleNode(xPath, null);
		        if (connectionNode == null)
		            connectionNode = parentNode.AppendChild(_settingsRootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "SavedSettings", null));
		        else
		        {
		            if (_databaseUserMetaMappings != null)
		            {
		                nodesToHack.Clear();
		                foreach (XmlNode childNode in connectionNode.ChildNodes)
		                {
		                    if (childNode.Name == "SavedXmlUserMetaAlias")
		                    {
		                        nodesToHack.Add(childNode);
		                    }
		                }
		                foreach (XmlNode node2kill in nodesToHack) connectionNode.RemoveChild(node2kill);
		            }
		        }

		        XmlHelper.SetAttribute(connectionNode, "name", inf.Name);
		        XmlHelper.SetAttribute(connectionNode, "driver", inf.Driver);
		        XmlHelper.SetAttribute(connectionNode, "connstr", inf.ConnectionString);
		        XmlHelper.SetAttribute(connectionNode, "dbtargetpath", inf.DbTargetPath);
		        XmlHelper.SetAttribute(connectionNode, "languagepath", inf.LanguagePath);
		        XmlHelper.SetAttribute(connectionNode, "usermetapath", inf.UserMetaDataPath);
		        XmlHelper.SetAttribute(connectionNode, "language", inf.Language);
		        XmlHelper.SetAttribute(connectionNode, "dbtarget", inf.DbTarget);
		        XmlHelper.SetAttribute(connectionNode, "defaultdatabaseonly", inf.ShowDefaultDatabaseOnly.ToString());

		        if (_databaseUserMetaMappings != null)
		        {
		            foreach (string dbkey in inf.DatabaseUserDataXmlMappings.Keys)
		            {
		                XmlNode mapXmlNode = connectionNode.AppendChild(_settingsRootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "SavedXmlUserMetaAlias", null));

		                XmlHelper.SetAttribute(mapXmlNode, "database", dbkey);
		                XmlHelper.SetAttribute(mapXmlNode, "alias", inf.DatabaseUserDataXmlMappings[dbkey]);
		            }
		        }
		    }

		    _savedConnections = null;
		}
        
		public Hashtable SavedConnections
		{
			get 
			{
			    if (_savedConnections != null) return _savedConnections;
			    _savedConnections = new Hashtable();

			    XmlNodeList nodes = _settingsRootNode.GetElementsByTagName("SavedSettings");
			    foreach (XmlNode node in nodes) 
			    {
			        try 
			        {
			            var connectionInfo = new ConnectionInfo(node);
			            _savedConnections[connectionInfo.Name] = connectionInfo;
			        }
			        catch {}
			    }

			    return _savedConnections; 
			}
		}
		#endregion

		#region Attributes
		public string Version
		{
		    get { return XmlHelper.GetAttribute(_settingsRootNode, "Version", MISSING); }
		    set { XmlHelper.SetAttribute(_settingsRootNode, "Version", value); }
		}
        #endregion

        #region Settings
        public string VersionRssUrl
        {
            get { return GetSetting("VersionRSSUrl", @"http://sourceforge.net/export/rss2_projfiles.php?group_id=198893"); }
            set { SetSetting("VersionRSSUrl", value); }
        }

        public bool ExecuteFromTemplateBrowserAsync
        {
            get { return GetSetting("TemplateBrowserExecAsync", true); }
            set { SetSetting("TemplateBrowserExecAsync", value.ToString()); }
        }

        public bool ShowDefaultDatabaseOnly
        {
            get { return GetSetting("ShowDefaultDatabaseOnly", false); }
            set { SetSetting("ShowDefaultDatabaseOnly", value.ToString()); }
        }

        public bool ConsoleWriteGeneratedDetails
        {
            get { return GetSetting("ConsoleWriteGeneratedDetails", false); }
            set { SetSetting("ConsoleWriteGeneratedDetails", value.ToString()); }
        }

	    public bool EnableDocumentStyleSettings
	    {
	        get { return GetSetting("DocStyleSettings", false); }
	        set { SetSetting("DocStyleSettings", value.ToString()); }
	    }

	    public string FontFamily
	    {
	        get { return GetSetting("FontFamily", ""); }
	        set { SetSetting("FontFamily", value); }
	    }

	    public string DbDriver
	    {
	        get { return GetSetting("DbDriver", "SQL").ToUpper(); }
	        set { SetSetting("DbDriver", value); }
	    }

	    public string ConnectionString
	    {
	        get { return GetSetting("ConnectionString", "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=sa;Data Source=localhost"); }
	        set { SetSetting("ConnectionString", value); }
	    }

	    public string LanguageMappingFile
	    {
	        get { return GetSetting("LanguageMappingFile", ApplicationPath + @"\Settings\Languages.xml"); }
	        set { SetSetting("LanguageMappingFile", value); }
	    }

	    public string Language
	    {
	        get { return GetSetting("Language", "C#"); }
	        set { SetSetting("Language", value); }
	    }

	    public string DbTargetMappingFile
	    {
	        get { return GetSetting("DbTargetMappingFile", ApplicationPath + @"\Settings\DbTargets.xml"); }
	        set { SetSetting("DbTargetMappingFile", value); }
	    }

	    public string DbTarget
	    {
	        get { return GetSetting("DbTarget", "SqlClient"); }
	        set { SetSetting("DbTarget", value); }
	    }

	    public string UserMetaDataFileName
	    {
	        get { return GetSetting("UserMetaDataFileName", UserDataPath + @"\Settings\UserMetaData.xml"); }
	        set { SetSetting("UserMetaDataFileName", value); }
	    }

	    public bool EnableLineNumbering
	    {
	        get { return GetSetting("EnableLineNumbering", false); }
	        set { SetSetting("EnableLineNumbering", value.ToString()); }
	    }

	    public bool EnableClipboard
	    {
	        get { return GetSetting("EnableClipboard", true); }
	        set { SetSetting("EnableClipboard", value.ToString()); }
	    }

	    public int CodePage
        {
            get { return GetSetting("CodePage",-1); }
            set { SetSetting("CodePage", value.ToString()); }
        }

		public int Tabs
		{
		    get { return GetSetting("Tabs", 4); }
		    set { SetSetting("Tabs", value.ToString()); }
		}

	    public int ScriptTimeout
	    {
	        get { return GetSetting("ScriptTimeout", -1); }
	        set { SetSetting("ScriptTimeout", value.ToString()); }
	    }

	    public Dictionary<string, string> DatabaseUserDataXmlMappings
	    {
	        get
	        {
	            if (_databaseUserMetaMappings != null) return _databaseUserMetaMappings;

	            _databaseUserMetaMappings = new Dictionary<string, string>();

	            XmlNodeList nodes = _settingsRootNode.GetElementsByTagName("XmlUserMetaDatabaseAlias");
	            foreach (XmlNode xmlDbAlias in nodes)
	            {
	                var db = XmlHelper.GetAttribute(xmlDbAlias, "database", "");
	                var alias = XmlHelper.GetAttribute(xmlDbAlias, "alias", "");

	                DatabaseUserDataXmlMappings[db] = alias;
	            }

	            return _databaseUserMetaMappings;
	        }
	    }

	    public string DatabaseUserDataXmlMappingsString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                foreach (string key in DatabaseUserDataXmlMappings.Keys)
                {
                    if (sb.Length > 0) sb.Append(",");
                    sb.Append(key).Append("=").Append(DatabaseUserDataXmlMappings[key]);
                }
                return sb.ToString();
            }
            set
            {
                DatabaseUserDataXmlMappings.Clear();
                string[] combpairs = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string pair in combpairs)
                {
                    string[] keyVal = pair.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);

                    if (keyVal.Length == 2)
                        DatabaseUserDataXmlMappings[keyVal[0]] = keyVal[1];
                }
            }
        }

        public string UserTemplateDirectory
        {
#if RUN_AS_NON_ADMIN        
            get
            {
                string result = GetSetting("UserTemplateDirectory");
                if ((MISSING == result) || (result == string.Empty))
                {
                    result = UserDataPath + @"\Templates\";
                    if (!Directory.Exists(result))
                        result = UserDataPath;

                    UserTemplateDirectory = result;
                }

                return result;
            }
            set { SetSetting("UserTemplateDirectory", value); }
#else
            get { return DefaultTemplateDirectory; }
            set { DefaultTemplateDirectory =value; }
#endif
        }

        public string DefaultTemplateDirectory
		{
			get 
            {
                string result = GetSetting("DefaultTemplateDirectory");
                if ((MISSING == result) || (result == string.Empty))
                {
                    result = ApplicationPath + @"\Templates\";
                    if (!Directory.Exists(result))
                        result = ApplicationPath;

                    DefaultTemplateDirectory = result;
                }

                return result; 
            }
			set	{ SetSetting("DefaultTemplateDirectory", value); }
		}

		public string DefaultOutputDirectory
		{
			get 
            {                 
                string result = GetSetting("DefaultOutputDirectory");
                if (MISSING == result)
                {
                    result = UserDataPath + @"\GeneratedCode\";
                    if (!Directory.Exists(result))
                        result = UserDataPath;

                    DefaultOutputDirectory = result;
                }

                return result;
            }
			set	{ SetSetting("DefaultOutputDirectory", value); }
		}

		public bool UseProxyServer
		{
			get 
			{ 
				return GetSetting("UseProxyServer",false); 
			}
			set	
			{ 
				SetSetting("UseProxyServer", value.ToString()); 
			}
		}

		public string ProxyServerUri
		{
			get 
			{
				return GetSetting("ProxyServerUri",string.Empty);
			}
			set	{ SetSetting("ProxyServerUri", value); }
		}

		public string ProxyAuthUsername
		{
			get 
			{
                return GetSetting("ProxyAuthUsername", string.Empty);
			}
			set	{ SetSetting("ProxyAuthUsername", value); }
		}

		public string ProxyAuthPassword
		{
			get 
			{
                return GetSetting("ProxyAuthPassword", string.Empty);
			}
			set	{ SetSetting("ProxyAuthPassword", value); }
		}

		public string ProxyAuthDomain
		{
			get 
			{
                return GetSetting("ProxyAuthDomain", string.Empty);
			}
			set	{ SetSetting("ProxyAuthDomain", value); }
		}

        public System.Net.WebProxy WebProxy
        {
            get
            {
                System.Net.WebProxy proxy = null;
                if (UseProxyServer)
                {
                    if (ProxyAuthUsername != string.Empty)
                    {
                        System.Net.NetworkCredential creds = new System.Net.NetworkCredential(ProxyAuthUsername, ProxyAuthPassword);
                        if (ProxyAuthDomain != string.Empty)
                        {
                            creds.Domain = ProxyAuthDomain;
                        }
                        proxy = new System.Net.WebProxy(ProxyServerUri, true, null, creds);
                    }

                    else
                    {
                        proxy = new System.Net.WebProxy(ProxyServerUri, true, null);
                    }
                }
                return proxy;
            }
        }

		public string WindowState
		{
            get { return GetSetting("WindowState", "Normal"); }
			set	{ SetSetting("WindowState", value); }
		}

		public string WindowPosTop
		{
            get { return GetSetting("WindowPosTop", "50"); }
			set	{ SetSetting("WindowPosTop", value); }
		}

		public string WindowPosLeft
		{
            get { return GetSetting("WindowPosLeft", "50"); }
			set	{ SetSetting("WindowPosLeft", value); }
		}

		public string WindowPosWidth
		{
            get { return GetSetting("WindowPosWidth", "800"); }
			set	{ SetSetting("WindowPosWidth", value); }
		}

		public string WindowPosHeight
		{
            get { return GetSetting("WindowPosHeight", "600"); }
			set	{ SetSetting("WindowPosHeight", value); }
		}

		public bool CheckForNewBuild
		{
			get 
			{ 
				return GetSetting("CheckForNewBuild", true);
			}

			set	{ SetSetting("CheckForNewBuild", value.ToString()); }
		}

		public bool CompactMemoryOnStartup
		{
			get 
			{ 
				return GetSetting("CompactMemoryOnStartup", true);
			}

			set	{ SetSetting("CompactMemoryOnStartup", value.ToString()); }
		}

		public bool DomainOverride
		{
			get 
			{ 
				// This is true by default
				return GetSetting("DomainOverride", true);
			}
			set	
			{ 
				SetSetting("DomainOverride", value.ToString()); 
			}
		}

        // k3b: to allow MyGen to run as a non admin it should *not* write into C:\Program Files\
        private static string UserDataPath
        {
            get
            {
#if RUN_AS_NON_ADMIN
                if (_userDataPath == null) // Load OnDemand
                {
                    _userDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                                            , "MyGeneration");

                    if (!Directory.Exists(_userDataPath))
                    {
                        Directory.CreateDirectory(_userDataPath);
                        Directory.CreateDirectory(Path.Combine(_userDataPath, "Settings"));
                        Directory.CreateDirectory(Path.Combine(_userDataPath, "Templates"));
                    }
                }
                return _userDataPath;
                    
#else
                return ApplicationPath; // not implemented yet. Feature disabled until then
#endif
            }
        }
        #endregion
		
		public void PopulateZeusContext(IZeusContext context) 
		{
			var settings = new DefaultSettings();
			IZeusInput input = context.Input;

			if (!input.Contains("__version"))
			{
				Assembly ver = Assembly.GetEntryAssembly();
                if (ver != null)
                {
                    input["__version"] = ver.GetName().Version.ToString();
                }
			}
			
			//-- BEGIN LEGACY VARIABLE SUPPORT -----
			if (!input.Contains("defaultTemplatePath")) 
				input["defaultTemplatePath"] = settings.DefaultTemplateDirectory;
			if (!input.Contains("defaultOutputPath")) 
				input["defaultOutputPath"] = settings.DefaultOutputDirectory;
			//-- END LEGACY VARIABLE SUPPORT -------

			if (!input.Contains("__defaultTemplatePath")) 
				input["__defaultTemplatePath"] = settings.DefaultTemplateDirectory;

			if (!input.Contains("__defaultOutputPath")) 
				input["__defaultOutputPath"] = settings.DefaultOutputDirectory;

			if (settings.DbDriver != string.Empty) 
			{
				//-- BEGIN LEGACY VARIABLE SUPPORT -----
				if (!input.Contains("dbDriver")) 
					input["dbDriver"] = settings.DbDriver;
				if (!input.Contains("dbConnectionString")) 
					input["dbConnectionString"] = settings.DomainOverride;
				//-- END LEGACY VARIABLE SUPPORT -------

				if (!input.Contains("__dbDriver"))
					input["__dbDriver"] = settings.DbDriver;
				
				if (!input.Contains("__dbConnectionString"))
					input["__dbConnectionString"] = settings.ConnectionString;

                if (!input.Contains("__showDefaultDatabaseOnly"))
                    input["__showDefaultDatabaseOnly"] = settings.ShowDefaultDatabaseOnly.ToString();
			
				if (!input.Contains("__domainOverride"))
					input["__domainOverride"] = settings.DomainOverride;

				if ( (settings.DbTarget != string.Empty) && (!input.Contains("__dbTarget")) )
					input["__dbTarget"] = settings.DbTarget;
			
				if ( (settings.DbTargetMappingFile != string.Empty) && (!input.Contains("__dbTargetMappingFileName")) ) 
					input["__dbTargetMappingFileName"] = settings.DbTargetMappingFile;

				if ( (settings.LanguageMappingFile != string.Empty) && (!input.Contains("__dbLanguageMappingFileName")) )
					input["__dbLanguageMappingFileName"] = settings.LanguageMappingFile;

				if ( (settings.Language != string.Empty) && (!input.Contains("__language")) )
					input["__language"] = settings.Language;

				if ( (settings.UserMetaDataFileName != string.Empty) && (!input.Contains("__userMetaDataFileName")) )
					input["__userMetaDataFileName"] = settings.UserMetaDataFileName;

                if (settings.DatabaseUserDataXmlMappings.Count > 0)
                {
                    foreach (string key in settings.DatabaseUserDataXmlMappings.Keys)
                    {
                        input["__dbmap__" + key] = settings.DatabaseUserDataXmlMappings[key];
                    }
                }
			}
		}

        public void PopulateDictionary(Dictionary<string, string> dict)
        {
            var settings = new DefaultSettings();

            if (!dict.ContainsKey("__version"))
            {
                Assembly ver = Assembly.GetEntryAssembly();
                if (ver != null)
                {
                    dict["__version"] = ver.GetName().Version.ToString();
                }
            }

            //-- BEGIN LEGACY VARIABLE SUPPORT -----
            dict["defaultTemplatePath"] = settings.DefaultTemplateDirectory;
            dict["defaultOutputPath"] = settings.DefaultOutputDirectory;
            //-- END LEGACY VARIABLE SUPPORT -------

            dict["__defaultTemplatePath"] = settings.DefaultTemplateDirectory;

            dict["__defaultOutputPath"] = settings.DefaultOutputDirectory;

            if (settings.DbDriver != string.Empty)
            {
                //-- BEGIN LEGACY VARIABLE SUPPORT -----
                dict["dbDriver"] = settings.DbDriver;
                dict["dbConnectionString"] = settings.DomainOverride.ToString();
                //-- END LEGACY VARIABLE SUPPORT -------

                dict["__dbDriver"] = settings.DbDriver;

                dict["__dbConnectionString"] = settings.ConnectionString;

                dict["__domainOverride"] = settings.DomainOverride.ToString();

                dict["__showDefaultDatabaseOnly"] = settings.ShowDefaultDatabaseOnly.ToString();

                if (settings.DbTarget != string.Empty)
                    dict["__dbTarget"] = settings.DbTarget;

                if (settings.DbTargetMappingFile != string.Empty)
                    dict["__dbTargetMappingFileName"] = settings.DbTargetMappingFile;

                if (settings.LanguageMappingFile != string.Empty)
                    dict["__dbLanguageMappingFileName"] = settings.LanguageMappingFile;

                if (settings.Language != string.Empty)
                    dict["__language"] = settings.Language;

                if (settings.UserMetaDataFileName != string.Empty)
                    dict["__userMetaDataFileName"] = settings.UserMetaDataFileName;

                if (settings.DatabaseUserDataXmlMappings.Count > 0)
                {
                    foreach (string key in settings.DatabaseUserDataXmlMappings.Keys)
                    {
                        dict["__dbmap__" + key] = settings.DatabaseUserDataXmlMappings[key];
                    }
                }
            }
        }

		#region Internal Stuff
		private string GetSetting(string name)
		{
			string xPath = @"Setting[@Name='" + name + "']";
            XmlNode node = _settingsRootNode.SelectSingleNode(xPath, null);

            return XmlHelper.GetAttribute(node,"value",MISSING);
		}

        public string GetSetting(string name, string notFoundValue)
        {
            string result = GetSetting(name);
	        if (result != MISSING) 
                return result;
            return notFoundValue;
        }

        public bool GetSetting(string name, bool notFoundValue)
        {
            bool result;
            if (bool.TryParse(GetSetting(name), out result))
                return result;
            return notFoundValue;
        }

        public int GetSetting(string name, int notFoundValue)
        {
            int result;
            if (int.TryParse(GetSetting(name), out result))
                return result;
            return notFoundValue;
        }

        public void SetSetting(string name, string data)
		{
			var xPath = @"Setting[@Name='" + name + "']";
            XmlNode node = _settingsRootNode.SelectSingleNode(xPath, null);

			if(node != null)
			{
				node.Attributes["value"].Value = data;
			}
			else
			{
				AddSetting(name, data);
			}
		}

		private void AddSetting(string name, string data)
		{
            if (_settingsRootNode == null) return;

		    XmlNode setting = _settingsRootNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Setting", null);

		    XmlAttribute xmlAttribute = _settingsRootNode.OwnerDocument.CreateAttribute("Name", null);
		    xmlAttribute.Value = name;
		    setting.Attributes.Append(xmlAttribute);

		    xmlAttribute = _settingsRootNode.OwnerDocument.CreateAttribute("value", null);
		    xmlAttribute.Value = data;
		    setting.Attributes.Append(xmlAttribute);

		    _settingsRootNode.AppendChild(setting);
		}
		#endregion
	}
}
