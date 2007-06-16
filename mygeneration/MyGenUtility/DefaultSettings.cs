using System;
using System.Xml;
using System.Text;
using System.IO;
using System.Reflection;
using System.Collections;
using Zeus;

namespace MyGeneration
{
	/// <summary>
	/// Summary description for DefaultSettings.
	/// </summary>
	public class DefaultSettings
	{
		private const string MISSING = "*&?$%";
		private string _appPath;
		private Hashtable _savedConnections;
		private ArrayList _recentFiles;

		public DefaultSettings()
		{
			xmlDoc = new XmlDocument();

			Assembly asmblyMyGen = Assembly.GetEntryAssembly();
			string version = asmblyMyGen.GetName().Version.ToString();

			try
			{
				xmlDoc.Load(ApplicationPath + @"\Settings\DefaultSettings.xml");

				if(this.Version != version)
				{
					// Our Version has changed, write any new settings and their defaults
					this.FillInMissingSettings(version);
					xmlDoc.Save(ApplicationPath + @"\Settings\DefaultSettings.xml");
				}
			}
			catch
			{
				// Our file doesn't exist, let's create it
				StringBuilder defaultXML = new StringBuilder();
				defaultXML.Append(@"<?xml version='1.0' encoding='utf-8'?>");
				defaultXML.Append(@"<DefaultSettings Version='" + version + "' FirstTime='true'>");
				defaultXML.Append(@"</DefaultSettings>");

				xmlDoc.LoadXml(defaultXML.ToString());
				this.FillInMissingSettings(version);

				xmlDoc.Save(ApplicationPath + @"\Settings\DefaultSettings.xml");
			}
		}

		private void FillInMissingSettings(string version)
		{
			string settingsPath = ApplicationPath + @"\Settings\";

			// Version
			this.Version = version;

			// Connection
			if(MISSING == this.DbDriver)				this.DbDriver = "SQL";
			if(MISSING == this.ConnectionString)		this.ConnectionString = "Provider=SQLOLEDB.1;Persist Security Info=False;User ID=sa;Data Source=localhost";

			// MyMeta
			if(MISSING == this.DbTargetMappingFile)		this.DbTargetMappingFile  = settingsPath + "DbTargets.xml";
			if(MISSING == this.LanguageMappingFile)		this.LanguageMappingFile  = settingsPath + "Languages.xml";
			if(MISSING == this.UserMetaDataFileName)	this.UserMetaDataFileName = settingsPath + "UserMetaData.xml";			
			if(MISSING == this.Language)				this.Language = "C#";
			if(MISSING == this.DbTarget)				this.DbTarget = "SqlClient";

			// Scripting
			if(MISSING == this.GetSetting("EnableLineNumbering"))		this.EnableLineNumbering = false;
			if(MISSING == this.GetSetting("EnableClipboard"))			this.EnableClipboard = true;
			if(MISSING == this.GetSetting("Tabs"))						this.Tabs = 4;
			if(MISSING == this.GetSetting("ScriptTimeout"))				this.ScriptTimeout = -1;
			if(MISSING == this.GetSetting("CheckForNewBuild"))			this.CheckForNewBuild = true;
            if (MISSING == this.GetSetting("CompactMemoryOnStartup"))   this.CompactMemoryOnStartup = true;
            if (MISSING == this.GetSetting("CodePage"))                 this.CodePage = -1;
            if (MISSING == this.GetSetting("FontFamily"))               this.FontFamily = string.Empty;

			if(MISSING == this.DefaultTemplateDirectory)
			{
				string defaultTemplatePath = ApplicationPath + @"\Templates\";
				if (!Directory.Exists(defaultTemplatePath)) 
					defaultTemplatePath = ApplicationPath;

				this.DefaultTemplateDirectory = defaultTemplatePath;
			}

			if(MISSING == this.DefaultOutputDirectory)
			{
				string defaultOutputPath = ApplicationPath + @"\GeneratedCode\";
				if (!Directory.Exists(defaultOutputPath)) 
					defaultOutputPath = ApplicationPath;
				
				this.DefaultOutputDirectory = defaultOutputPath;
			}
		}

		public void Save()
		{
			this.UpdateSavedConnections();
			this.UpdateRecentFiles();

			xmlDoc.Save(ApplicationPath + @"\Settings\DefaultSettings.xml");
		}

		#region Recent Files Nodes
		public void UpdateRecentFiles() 
		{
			if (this._recentFiles != null)
			{
				string xPath = @"//DefaultSettings";
				XmlNode parentNode = xmlDoc.SelectSingleNode(xPath, null);

				ArrayList nodesToHack = new ArrayList();
				XmlNodeList nodes = xmlDoc.GetElementsByTagName("RecentFile");
				
				foreach (XmlNode node in nodes) nodesToHack.Add(node);
				foreach (XmlNode node in nodesToHack) parentNode.RemoveChild(node);

				int i = 0;
				foreach (string path in _recentFiles) 
				{
					XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, "RecentFile", null);
					node.InnerText = path;

					parentNode.AppendChild(node);

					if (++i > 20) break;
				}

				_recentFiles = null;
			}
		}

		public ArrayList RecentFiles
		{
			get 
			{ 
				if (_recentFiles == null) 
				{
					_recentFiles = new ArrayList();

					XmlNodeList nodes = this.xmlDoc.GetElementsByTagName("RecentFile");
					foreach (XmlNode node in nodes) 
					{
						if ((node.InnerText != null) && (node.InnerText != string.Empty)) 
						{
							if (File.Exists(node.InnerText))
							{
								_recentFiles.Add(node.InnerText);
							}
						}
					}
				}

				return _recentFiles; 
			}
		}
		#endregion

		#region Saved Connections Nodes
		public void UpdateSavedConnections() 
		{
			if (this._savedConnections != null)
			{
				string xPath = @"//DefaultSettings";
				XmlNode parentNode = xmlDoc.SelectSingleNode(xPath, null);

				ArrayList nodesToHack = new ArrayList();
				XmlNodeList nodes = xmlDoc.GetElementsByTagName("SavedSettings");
				foreach (XmlNode node in nodes) 
				{
					if (!this.SavedConnections.ContainsKey( node.Attributes["name"].Value ))
					{
						nodesToHack.Add(node);
					}
				}
				foreach (XmlNode node in nodesToHack) parentNode.RemoveChild(node);

				foreach (ConnectionInfo inf in _savedConnections.Values) 
				{
					xPath = @"//DefaultSettings/SavedSettings[@name='" + inf.Name.Replace("'", "&apos;") + "']";
					XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

					if(node != null)
					{
						//update node
						node.Attributes["driver"].Value = inf.Driver;
						node.Attributes["connstr"].Value = inf.ConnectionString;
						node.Attributes["dbtargetpath"].Value = inf.DbTargetPath;
						node.Attributes["languagepath"].Value = inf.LanguagePath;
						node.Attributes["usermetapath"].Value = inf.UserMetaDataPath;
						node.Attributes["language"].Value = inf.Language;
						node.Attributes["dbtarget"].Value = inf.DbTarget;
					}
					else
					{
						// add node
						node = xmlDoc.CreateNode(XmlNodeType.Element, "SavedSettings", null);

						node.Attributes.Append(xmlDoc.CreateAttribute("name"));
						node.Attributes.Append(xmlDoc.CreateAttribute("driver"));
						node.Attributes.Append(xmlDoc.CreateAttribute("connstr"));
						node.Attributes.Append(xmlDoc.CreateAttribute("dbtargetpath"));
						node.Attributes.Append(xmlDoc.CreateAttribute("languagepath"));
						node.Attributes.Append(xmlDoc.CreateAttribute("usermetapath"));
						node.Attributes.Append(xmlDoc.CreateAttribute("language"));
						node.Attributes.Append(xmlDoc.CreateAttribute("dbtarget"));

						node.Attributes["name"].Value = inf.Name;
						node.Attributes["driver"].Value = inf.Driver;
						node.Attributes["connstr"].Value = inf.ConnectionString;
						node.Attributes["dbtargetpath"].Value = inf.DbTargetPath;
						node.Attributes["languagepath"].Value = inf.LanguagePath;
						node.Attributes["usermetapath"].Value = inf.UserMetaDataPath;
						node.Attributes["language"].Value = inf.Language;
						node.Attributes["dbtarget"].Value = inf.DbTarget;

						parentNode.AppendChild( node );
					}
				}

				_savedConnections = null;
			}
		}

		public Hashtable SavedConnections
		{
			get 
			{ 
				if (_savedConnections == null) 
				{
					_savedConnections = new Hashtable();

					XmlNodeList nodes = this.xmlDoc.GetElementsByTagName("SavedSettings");
					foreach (XmlNode node in nodes) 
					{
						try 
						{
							ConnectionInfo inf = new ConnectionInfo(node);
							_savedConnections[inf.Name] = inf;
						}
						catch {}
					}
				}

				return _savedConnections; 
			}
		}
		#endregion

		#region Attributes on <DefaultSettings>
		public string Version
		{
			get
			{
				string version = MISSING;

				string xPath = @"//DefaultSettings";
				XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

				if(node != null)
				{
					XmlAttribute attr = node.Attributes["Version"];
					if(null != attr)
					{
						version = attr.Value;
					}
				}

				return version;
			}

			set
			{
				string xPath = @"//DefaultSettings";
				XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

				if(node != null)
				{
					XmlAttribute attr = node.Attributes["Version"];
					if(null != attr)
					{
						attr.Value = value;
					}
					else
					{
						attr = xmlDoc.CreateAttribute("Version", null);
						attr.Value = value;
						node.Attributes.Append(attr);
					}
				}
			}
		}

		public bool FirstLoad
		{
			get
			{
				bool first = false;

				string xPath = @"//DefaultSettings";
				XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

				if(node != null)
				{
					XmlAttribute attr = node.Attributes["FirstTime"];
					string firstTime = attr.Value as string;
					first = firstTime == "true" ? true : false;
				}

				return first;
			}

			set
			{
				string xPath = @"//DefaultSettings";
				XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

				if(node != null)
				{
					XmlAttribute attr = node.Attributes["FirstTime"];
					if(null != attr)
					{
						attr.Value = (value == true) ? "true" : "false";
					}
				}
			}
		}
        #endregion

        public string FontFamily
        {
            get { return this.GetSetting("FontFamily"); }
            set { this.SetSetting("FontFamily", value); }
        }

        public string DbDriver
        {
            get { return this.GetSetting("DbDriver").ToUpper(); }
            set { this.SetSetting("DbDriver", value); }
        }

		public string ConnectionString
		{
			get { return this.GetSetting("ConnectionString"); }
			set	{ this.SetSetting("ConnectionString", value); }
		}

		public string LanguageMappingFile
		{
			get { return this.GetSetting("LanguageMappingFile"); }
			set	{ this.SetSetting("LanguageMappingFile", value); }
		}

		public string Language
		{
			get { return this.GetSetting("Language"); }
			set	{ this.SetSetting("Language", value); }
		}

		public string DbTargetMappingFile
		{
			get { return this.GetSetting("DbTargetMappingFile"); }
			set	{ this.SetSetting("DbTargetMappingFile", value); }
		}

		public string DbTarget
		{
			get { return this.GetSetting("DbTarget"); }
			set	{ this.SetSetting("DbTarget", value); }
		}

		public string UserMetaDataFileName
		{
			get { return this.GetSetting("UserMetaDataFileName"); }
			set	{ this.SetSetting("UserMetaDataFileName", value); }
		}
		
		#region LastConnection Settings

		public string SQL
		{
			get 
			{ 
				string str = this.GetSetting("SQL"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("SQL", value); }
		}

		public string ACCESS
		{
			get 
			{ 
				string str = this.GetSetting("ACCESS"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("ACCESS", value); }
		}

		public string FIREBIRD
		{
			get 
			{ 
				string str = this.GetSetting("FIREBIRD"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("FIREBIRD", value); }
		}

		public string DB2
		{
			get 
			{ 
				string str = this.GetSetting("DB2"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("DB2", value); }
		}

		public string ISERIES
		{
			get 
			{ 
				string str = this.GetSetting("ISERIES"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("ISERIES", value); }
		}

		public string INTERBASE
		{
			get 
			{ 
				string str = this.GetSetting("INTERBASE"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("INTERBASE", value); }
		}

		public string MYSQL
		{
			get 
			{ 
				string str = this.GetSetting("MYSQL"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("MYSQL", value); }
		}

        public string GetPlugin(string driver)
        {
            string str = this.GetSetting("PLUGIN_" + CleanName(driver));
            return (str == MISSING) ? "" : str;	
        }

        public void SetPlugin(string driver, string value)
        {
            this.SetSetting("PLUGIN_" + CleanName(driver), value); 
        }

        private string CleanName(string name)
        {
            StringBuilder outName = new StringBuilder();
            foreach (char c in name)
            {
                if (Char.IsLetterOrDigit(c))
                {
                    outName.Append(c);
                }
            }
            return outName.ToString();
        }

		public string MYSQL2
		{
			get 
			{ 
				string str = this.GetSetting("MYSQL2"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("MYSQL2", value); }
		}

		public string PERVASIVE
		{
			get 
			{ 
				string str = this.GetSetting("PERVASIVE"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("PERVASIVE", value); }
		}


		public string POSTGRESQL
		{
			get 
			{ 
				string str = this.GetSetting("POSTGRESQL"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("POSTGRESQL", value); }
		}

		public string POSTGRESQL8
		{
			get 
			{ 
				string str = this.GetSetting("POSTGRESQL8"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("POSTGRESQL8", value); }
		}

		public string ORACLE
		{
			get 
			{ 
				string str = this.GetSetting("ORACLE"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("ORACLE", value); }
		}

		public string SQLITE
		{
			get 
			{ 
				string str = this.GetSetting("SQLITE"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("SQLITE", value); }
		}

		public string VISTADB
		{
			get 
			{ 
				string str = this.GetSetting("VISTADB"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("VISTADB", value); }
		}

		public string ADVANTAGE
		{
			get 
			{ 
				string str = this.GetSetting("ADVANTAGE"); 
				return (str == MISSING) ? "" : str;				
			}
			set	{ this.SetSetting("ADVANTAGE", value); }
		}

		#endregion

		public bool EnableLineNumbering
		{
			get { return Convert.ToBoolean(this.GetSetting("EnableLineNumbering")); }
			set	{ this.SetSetting("EnableLineNumbering", value.ToString()); }
		}

		public bool EnableClipboard
		{
			get { return Convert.ToBoolean(this.GetSetting("EnableClipboard")); }
			set	{ this.SetSetting("EnableClipboard", value.ToString()); }
        }

        public int CodePage
        {
            get { return Convert.ToInt32(this.GetSetting("CodePage")); }
            set { this.SetSetting("CodePage", value.ToString()); }
        }

		public int Tabs
		{
			get { return Convert.ToInt32(this.GetSetting("Tabs")); }
			set { this.SetSetting("Tabs", value.ToString()); }
		}

		public int ScriptTimeout
		{
			get { return Convert.ToInt32(this.GetSetting("ScriptTimeout")); }
			set { this.SetSetting("ScriptTimeout", value.ToString()); }
		}

		public string DefaultTemplateDirectory
		{
			get { return this.GetSetting("DefaultTemplateDirectory"); }
			set	{ this.SetSetting("DefaultTemplateDirectory", value); }
		}

		public string DefaultOutputDirectory
		{
			get { return this.GetSetting("DefaultOutputDirectory"); }
			set	{ this.SetSetting("DefaultOutputDirectory", value); }
		}

		public bool UseProxyServer
		{
			get 
			{ 
				string useproxy = this.GetSetting("UseProxyServer"); 
				if (useproxy == MISSING) return false;
				else return Convert.ToBoolean(useproxy);
			}
			set	
			{ 
				this.SetSetting("UseProxyServer", value.ToString()); 
			}
		}

		public string ProxyServerUri
		{
			get 
			{
				string uri = this.GetSetting("ProxyServerUri");
				if (uri == MISSING) 
				{
					return string.Empty;
				}
				else
				{
					return uri; 
				}
			}
			set	{ this.SetSetting("ProxyServerUri", value); }
		}

		public string ProxyAuthUsername
		{
			get 
			{
				string user = this.GetSetting("ProxyAuthUsername");
				if (user == MISSING) 
				{
					return string.Empty;
				}
				else
				{
					return user; 
				}
			}
			set	{ this.SetSetting("ProxyAuthUsername", value); }
		}

		public string ProxyAuthPassword
		{
			get 
			{
				string passwd = this.GetSetting("ProxyAuthPassword");
				if (passwd == MISSING) 
				{
					return string.Empty;
				}
				else
				{
					return passwd; 
				}
			}
			set	{ this.SetSetting("ProxyAuthPassword", value); }
		}

		public string ProxyAuthDomain
		{
			get 
			{
				string domain = this.GetSetting("ProxyAuthDomain");
				if (domain == MISSING) 
				{
					return string.Empty;
				}
				else
				{
					return domain; 
				}
			}
			set	{ this.SetSetting("ProxyAuthDomain", value); }
		}

		public string WindowState
		{
			get { return this.GetSetting("WindowState"); }
			set	{ this.SetSetting("WindowState", value); }
		}

		public string WindowPosTop
		{
			get { return this.GetSetting("WindowPosTop"); }
			set	{ this.SetSetting("WindowPosTop", value); }
		}

		public string WindowPosLeft
		{
			get { return this.GetSetting("WindowPosLeft"); }
			set	{ this.SetSetting("WindowPosLeft", value); }
		}

		public string WindowPosWidth
		{
			get { return this.GetSetting("WindowPosWidth"); }
			set	{ this.SetSetting("WindowPosWidth", value); }
		}

		public string WindowPosHeight
		{
			get { return this.GetSetting("WindowPosHeight"); }
			set	{ this.SetSetting("WindowPosHeight", value); }
		}

		public bool CheckForNewBuild
		{
			get 
			{ 
				string chk = this.GetSetting("CheckForNewBuild");

				if(chk == DefaultSettings.MISSING)
				{
					return false;
				}
				else
				{
					return Convert.ToBoolean(chk); 
				}
			}

			set	{ this.SetSetting("CheckForNewBuild", value.ToString()); }
		}

		public bool CompactMemoryOnStartup
		{
			get 
			{ 
				string chk = this.GetSetting("CompactMemoryOnStartup");

				if(chk == DefaultSettings.MISSING)
				{
					return true;
				}
				else
				{
					return Convert.ToBoolean(chk); 
				}
			}

			set	{ this.SetSetting("CompactMemoryOnStartup", value.ToString()); }
		}

		public bool DomainOverride
		{
			get 
			{ 
				// This is true by default
				string domain = this.GetSetting("DomainOverride");
				if (domain == MISSING) return true;
				else return Convert.ToBoolean(domain);
			}
			set	
			{ 
				this.SetSetting("DomainOverride", value.ToString()); 
			}
		}

		private string ApplicationPath 
		{
			get 
			{
				if (_appPath == null) 
				{
					_appPath = Assembly.GetEntryAssembly().Location;
					_appPath = _appPath.Substring(0, _appPath.LastIndexOf(@"\"));
				}
				return _appPath;
			}
		}

		
		public void PopulateZeusContext(IZeusContext context) 
		{
			DefaultSettings settings = new DefaultSettings();
			IZeusInput input = context.Input;

			if (!input.Contains("__version"))
			{
				Assembly ver = System.Reflection.Assembly.GetEntryAssembly();
				input["__version"] = ver.GetName().Version.ToString();
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
			}
		}

		#region Internal Stuff
		protected string GetSetting(string name)
		{
			string xPath = @"//DefaultSettings/Setting[@Name='" + name + "']";
			XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

			if(node != null)
			{
				return node.Attributes["value"].Value;
			}
			else
			{
				return MISSING;
			}
		}

		protected void SetSetting(string name, string data)
		{
			string xPath = @"//DefaultSettings/Setting[@Name='" + name + "']";
			XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

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
			string xPath = @"//DefaultSettings";
			XmlNode node = xmlDoc.SelectSingleNode(xPath, null);

			if(node != null)
			{
				XmlAttribute attr;
				XmlNode setting = xmlDoc.CreateNode(XmlNodeType.Element, "Setting", null);

				attr = xmlDoc.CreateAttribute("Name", null);
				attr.Value = name;
				setting.Attributes.Append(attr);

				attr = xmlDoc.CreateAttribute("value", null);
				attr.Value = data;
				setting.Attributes.Append(attr);

				node.AppendChild(setting);
			}
		}
		#endregion

		private XmlDocument xmlDoc;
	}

	public class ConnectionInfo 
	{
		public string Name;
		public string Driver;
		public string ConnectionString;
		public string DbTarget;
		public string DbTargetPath;
		public string Language;
		public string LanguagePath;
		public string UserMetaDataPath;

		public ConnectionInfo() {}

		public ConnectionInfo(XmlNode node) 
		{
			Name = node.Attributes["name"].Value;
			Driver = node.Attributes["driver"].Value;
			ConnectionString = node.Attributes["connstr"].Value;
			DbTargetPath = node.Attributes["dbtargetpath"].Value;
			LanguagePath = node.Attributes["languagepath"].Value;
			UserMetaDataPath = node.Attributes["usermetapath"].Value;
			DbTarget = node.Attributes["dbtarget"].Value;
			Language = node.Attributes["language"].Value;
		}

		public override string ToString()
		{
			return this.Name;
		}

	}
}
